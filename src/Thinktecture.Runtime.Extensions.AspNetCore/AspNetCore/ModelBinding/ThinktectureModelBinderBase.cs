using System.ComponentModel;
using System.Runtime.ExceptionServices;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Thinktecture.AspNetCore.ModelBinding;

/// <summary>
/// Model binder for implementations Smart Enums and Value Objects with a key member.
/// </summary>
/// <typeparam name="T">Type of the value object.</typeparam>
/// <typeparam name="TKey">Type of the key member.</typeparam>
/// <typeparam name="TValidationError">Type of the validation error.</typeparam>
[ThinktectureRuntimeExtensionInternal]
public abstract class ThinktectureModelBinderBase<T, TKey, TValidationError> : IModelBinder
   where T : IObjectFactory<T, TKey, TValidationError>
   where TKey : notnull
   where TValidationError : class, IValidationError<TValidationError>
{
   private static readonly Type _type = typeof(T);
   private static readonly Type _keyType = typeof(TKey);
   private static readonly bool _disallowDefaultValues = typeof(IDisallowDefaultValue).IsAssignableFrom(typeof(T));

   private readonly TypeConverter? _keyConverter;

   /// <summary>
   /// Initializes a new instance of <see cref="ThinktectureModelBinderBase{T,TKey,TValidationError}"/>.
   /// </summary>
   private protected ThinktectureModelBinderBase()
   {
      var converter = TypeDescriptor.GetConverter(typeof(TKey));
      _keyConverter = converter.CanConvertFrom(typeof(string)) ? converter : null;
   }

   /// <inheritdoc />
   public Task BindModelAsync(ModelBindingContext bindingContext)
   {
      ArgumentNullException.ThrowIfNull(bindingContext);

      var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
      bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

      try
      {
         var value = valueProviderResult.FirstValue;

         if (bindingContext.ModelMetadata.ConvertEmptyStringToNull)
            value = value.TrimOrNullify();

         if (value is null)
         {
            var isNullable = Nullable.GetUnderlyingType(bindingContext.ModelType) == _type;

            if (isNullable || (bindingContext.ModelType.IsClass && !_disallowDefaultValues))
            {
               bindingContext.Result = ModelBindingResult.Success(null);
               bindingContext.ModelState.MarkFieldValid(bindingContext.ModelName);
               return Task.CompletedTask;
            }
         }

         object? key;

         if (_keyType == typeof(string))
         {
            key = value;
         }
         else
         {
            if (_keyConverter is null)
            {
               bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, $"Cannot convert a string to type \"{typeof(T).Name}\".");
               return Task.CompletedTask;
            }

            if (value is null && _keyType.IsValueType)
            {
               bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, $"Cannot convert null to type \"{typeof(T).Name}\".");
               return Task.CompletedTask;
            }

            key = _keyConverter.ConvertFrom(
               null,
               valueProviderResult.Culture,
               value!);
         }

         if (key is null)
         {
            if (_disallowDefaultValues)
            {
               bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, $"Cannot convert null to type \"{typeof(T).Name}\" because it doesn't allow default values.");
               return Task.CompletedTask;
            }

            bindingContext.Result = ModelBindingResult.Success(default(T));
            bindingContext.ModelState.MarkFieldValid(bindingContext.ModelName);
            return Task.CompletedTask;
         }

         var validationError = T.Validate((TKey)key, valueProviderResult.Culture, out var obj);

         if (validationError is null)
         {
            bindingContext.ModelState.MarkFieldValid(bindingContext.ModelName);
            bindingContext.Result = ModelBindingResult.Success(obj);
         }
         else
         {
            bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, validationError.ToString() ?? $"There is no item of type '{typeof(T).Name}' with the identifier '{key}'.");
         }

         return Task.CompletedTask;
      }
      catch (Exception exception)
      {
         var isFormatException = exception is FormatException;

         if (!isFormatException && exception.InnerException != null)
         {
            // TypeConverter throws System.Exception wrapping the FormatException, so we capture the inner exception.
            exception = ExceptionDispatchInfo.Capture(exception.InnerException).SourceException;
         }

         bindingContext.ModelState.TryAddModelError(
            bindingContext.ModelName,
            exception,
            bindingContext.ModelMetadata);

         // Were able to find a converter for the type but conversion failed.
         return Task.CompletedTask;
      }
   }
}

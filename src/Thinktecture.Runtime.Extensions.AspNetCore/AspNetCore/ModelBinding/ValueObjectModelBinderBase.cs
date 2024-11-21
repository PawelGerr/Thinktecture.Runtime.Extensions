using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Logging;

namespace Thinktecture.AspNetCore.ModelBinding;

/// <summary>
/// Model binder for implementations Smart Enums and Value Objects with a key member.
/// </summary>
/// <typeparam name="T">Type of the value object.</typeparam>
/// <typeparam name="TKey">Type of the key member.</typeparam>
/// <typeparam name="TValidationError">Type of the validation error.</typeparam>
public abstract class ValueObjectModelBinderBase<T, TKey, TValidationError> : SimpleTypeModelBinder
   where T : IValueObjectFactory<T, TKey, TValidationError>
   where TKey : notnull
   where TValidationError : class, IValidationError<TValidationError>
{
   private static readonly bool _mayReturnInvalidObjects = typeof(IValidatableEnum).IsAssignableFrom(typeof(T));

   /// <summary>
   /// Initializes a new instance of <see cref="ValueObjectModelBinder{T,TKey,TValidationError}"/>.
   /// </summary>
   /// <param name="loggerFactory">Logger factory.</param>
   protected ValueObjectModelBinderBase(
      ILoggerFactory loggerFactory)
      : base(typeof(TKey), loggerFactory)
   {
   }

   /// <inheritdoc />
   protected override void CheckModel(ModelBindingContext bindingContext, ValueProviderResult valueProviderResult, object? model)
   {
      if (model is not TKey key)
      {
         base.CheckModel(bindingContext, valueProviderResult, model);
         return;
      }

      key = Prepare(key);
      var validationError = T.Validate(key, valueProviderResult.Culture, out var obj);

      if (validationError is null || _mayReturnInvalidObjects)
      {
         bindingContext.Result = ModelBindingResult.Success(obj);
         return;
      }

      bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, validationError.ToString() ?? $"There is no item of type '{typeof(T).Name}' with the identifier '{key}'.");
   }

   /// <summary>
   /// Prepares the key before validation.
   /// </summary>
   /// <param name="key">Key to prepare.</param>
   /// <returns>Prepared key.</returns>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   protected abstract TKey Prepare(TKey key);
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Thinktecture.Internal;

namespace Thinktecture.AspNetCore.ModelBinding
{
   /// <summary>
   /// Model binder for implementations of <see cref="IEnum{TKey}"/> and value types with a key member.
   /// </summary>
   /// <typeparam name="T">Type of the value type.</typeparam>
   /// <typeparam name="TKey">Type of the key member.</typeparam>
   public class ValueTypeModelBinder<T, TKey> : IModelBinder
      where TKey : notnull
   {
      private readonly Validate<T, TKey> _validate;
      private readonly TypeConverter _keyConverter;

      /// <summary>
      /// Initializes a new instance of <see cref="ValueTypeModelBinder{T,TKey}"/>.
      /// </summary>
      /// <param name="validate">Callback that performs the actual binding.</param>
      public ValueTypeModelBinder(Validate<T, TKey> validate)
      {
         _validate = validate ?? throw new ArgumentNullException(nameof(validate));
         _keyConverter = TypeDescriptor.GetConverter(typeof(TKey));
      }

      /// <inheritdoc />
      public Task BindModelAsync(ModelBindingContext bindingContext)
      {
         if (bindingContext is null)
            throw new ArgumentNullException(nameof(bindingContext));

         var modelName = bindingContext.ModelName;
         var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

         if (valueProviderResult == ValueProviderResult.None)
            return Task.CompletedTask;

         bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

         var value = valueProviderResult.FirstValue;

         if (String.IsNullOrEmpty(value))
            return Task.CompletedTask;

         if (value is not TKey key)
         {
            key = (TKey)(_keyConverter.ConvertFrom(null!, valueProviderResult.Culture, value)
                         ?? throw new NotSupportedException($"Could not convert to value '{value}' to type '{typeof(TKey).Name}'."));
         }

         var validationResult = _validate(key, out var model);

         if (validationResult != ValidationResult.Success)
         {
            bindingContext.ModelState.TryAddModelError(modelName, validationResult!.ErrorMessage ?? $"The value '{value}' is not valid.");
            return Task.CompletedTask;
         }

         bindingContext.Result = ModelBindingResult.Success(model);

         return Task.CompletedTask;
      }
   }
}

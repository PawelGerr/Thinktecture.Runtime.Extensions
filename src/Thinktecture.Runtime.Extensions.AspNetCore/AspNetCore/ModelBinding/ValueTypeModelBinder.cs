using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Logging;
using Thinktecture.Internal;

namespace Thinktecture.AspNetCore.ModelBinding
{
   /// <summary>
   /// Model binder for implementations of <see cref="IEnum{TKey}"/> and value types with a key member.
   /// </summary>
   /// <typeparam name="T">Type of the value type.</typeparam>
   /// <typeparam name="TKey">Type of the key member.</typeparam>
   public class ValueTypeModelBinder<T, TKey> : SimpleTypeModelBinder
      where TKey : notnull
   {
      private readonly Validate<T, TKey> _validate;

      /// <summary>
      /// Initializes a new instance of <see cref="ValueTypeModelBinder{T,TKey}"/>.
      /// </summary>
      /// <param name="loggerFactory">Logger factory.</param>
      /// <param name="validate">Callback that performs the actual binding.</param>
      public ValueTypeModelBinder(
         ILoggerFactory loggerFactory,
         Validate<T, TKey> validate)
         : base(typeof(TKey), loggerFactory)
      {
         _validate = validate ?? throw new ArgumentNullException(nameof(validate));
      }

      /// <inheritdoc />
      protected override void CheckModel(ModelBindingContext bindingContext, ValueProviderResult valueProviderResult, object? model)
      {
         if (model is TKey key)
         {
            var validationResult = _validate(key, out var obj);

            if (validationResult != ValidationResult.Success)
            {
               bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, validationResult!.ErrorMessage ?? $"The value '{obj}' is not valid.");
            }
            else
            {
               bindingContext.Result = ModelBindingResult.Success(obj);
            }
         }
         else
         {
            base.CheckModel(bindingContext, valueProviderResult, model);
         }
      }
   }
}

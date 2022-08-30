using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Logging;
using Thinktecture.Internal;

namespace Thinktecture.AspNetCore.ModelBinding;

/// <summary>
/// Model binder for implementations of <see cref="IEnum{TKey}"/> and Value Objects with a key member.
/// </summary>
/// <typeparam name="T">Type of the value object.</typeparam>
/// <typeparam name="TKey">Type of the key member.</typeparam>
public class ValueObjectModelBinder<T, TKey> : SimpleTypeModelBinder
   where TKey : notnull
{
   private readonly Validate<T, TKey> _validate;

   /// <summary>
   /// Initializes a new instance of <see cref="ValueObjectModelBinder{T,TKey}"/>.
   /// </summary>
   /// <param name="loggerFactory">Logger factory.</param>
   /// <param name="validate">Callback that performs the actual binding.</param>
   public ValueObjectModelBinder(
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
         key = Prepare(key);
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

   /// <summary>
   /// Prepares the key before validation.
   /// </summary>
   /// <param name="key">Key to prepare.</param>
   /// <returns>Prepared key.</returns>
   protected virtual TKey Prepare(TKey key)
   {
      return key;
   }
}

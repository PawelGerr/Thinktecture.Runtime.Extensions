using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace Thinktecture.AspNetCore.ModelBinding;

/// <summary>
/// Model binder for implementations of <see cref="IKeyedValueObject{TKey}"/> and Value Objects with a key member.
/// </summary>
/// <typeparam name="T">Type of the value object.</typeparam>
/// <typeparam name="TKey">Type of the key member.</typeparam>
/// <typeparam name="TValidationError">Type of the validation error.</typeparam>
public sealed class ValueObjectModelBinder<T, TKey, TValidationError> : ValueObjectModelBinderBase<T, TKey, TValidationError>
   where T : IValueObjectFactory<T, TKey, TValidationError>
   where TKey : notnull
   where TValidationError : class, IValidationError<TValidationError>
{
   /// <summary>
   /// Initializes a new instance of <see cref="ValueObjectModelBinder{T,TKey,TValidationError}"/>.
   /// </summary>
   /// <param name="loggerFactory">Logger factory.</param>
   public ValueObjectModelBinder(
      ILoggerFactory loggerFactory)
      : base(loggerFactory)
   {
   }

   /// <summary>
   /// Prepares the key before validation.
   /// </summary>
   /// <param name="key">Key to prepare.</param>
   /// <returns>Prepared key.</returns>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   protected override TKey Prepare(TKey key)
   {
      return key;
   }
}

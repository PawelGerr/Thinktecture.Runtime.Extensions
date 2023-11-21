using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace Thinktecture.AspNetCore.ModelBinding;

/// <summary>
/// Model binder for implementations of string-based Smart Enums Value Objects with a key member.
/// </summary>
/// <typeparam name="T">Type of the value object.</typeparam>
/// <typeparam name="TValidationError">Type of the validation error.</typeparam>
public sealed class TrimmingSmartEnumModelBinder<T, TValidationError> : ValueObjectModelBinderBase<T, string, TValidationError>
   where T : IValueObjectFactory<T, string, TValidationError>
   where TValidationError : class, IValidationError<TValidationError>
{
   /// <summary>
   /// Initializes a new instance of <see cref="ValueObjectModelBinder{T,TKey,TValidationError}"/>.
   /// </summary>
   /// <param name="loggerFactory">Logger factory.</param>
   public TrimmingSmartEnumModelBinder(
      ILoggerFactory loggerFactory)
      : base(loggerFactory)
   {
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   protected override string Prepare(string key)
   {
      return key.Trim();
   }
}

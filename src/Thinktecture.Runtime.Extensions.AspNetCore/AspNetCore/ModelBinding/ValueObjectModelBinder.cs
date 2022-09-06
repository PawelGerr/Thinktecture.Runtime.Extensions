using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Thinktecture.Internal;

namespace Thinktecture.AspNetCore.ModelBinding;

/// <summary>
/// Model binder for implementations of <see cref="IEnum{TKey}"/> and Value Objects with a key member.
/// </summary>
/// <typeparam name="T">Type of the value object.</typeparam>
/// <typeparam name="TKey">Type of the key member.</typeparam>
public sealed class ValueObjectModelBinder<T, TKey> : ValueObjectModelBinderBase<T, TKey>
   where TKey : notnull
{
   /// <summary>
   /// Initializes a new instance of <see cref="ValueObjectModelBinder{T,TKey}"/>.
   /// </summary>
   /// <param name="loggerFactory">Logger factory.</param>
   /// <param name="validate">Callback that performs the actual binding.</param>
   public ValueObjectModelBinder(
      ILoggerFactory loggerFactory,
      Validate<T, TKey> validate)
      : base(loggerFactory, validate)
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

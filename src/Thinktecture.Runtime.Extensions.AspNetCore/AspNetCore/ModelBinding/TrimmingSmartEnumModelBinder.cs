using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace Thinktecture.AspNetCore.ModelBinding;

/// <summary>
/// Model binder for implementations of string-based <see cref="IKeyedValueObject{TKey}"/>.
/// </summary>
/// <typeparam name="T">Type of the value object.</typeparam>
public sealed class TrimmingSmartEnumModelBinder<T> : ValueObjectModelBinderBase<T, string>
   where T : IValueObjectFactory<T, string>
{
   /// <summary>
   /// Initializes a new instance of <see cref="ValueObjectModelBinder{T,TKey}"/>.
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

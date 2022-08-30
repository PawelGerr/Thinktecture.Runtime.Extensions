using Microsoft.Extensions.Logging;
using Thinktecture.Internal;

namespace Thinktecture.AspNetCore.ModelBinding;

/// <summary>
/// Model binder for implementations of string-based <see cref="IEnum{TKey}"/>.
/// </summary>
/// <typeparam name="T">Type of the value object.</typeparam>
public sealed class TrimmingSmartEnumModelBinder<T> : ValueObjectModelBinder<T, string>
{
   /// <summary>
   /// Initializes a new instance of <see cref="ValueObjectModelBinder{T,TKey}"/>.
   /// </summary>
   /// <param name="loggerFactory">Logger factory.</param>
   /// <param name="validate">Callback that performs the actual binding.</param>
   public TrimmingSmartEnumModelBinder(
      ILoggerFactory loggerFactory,
      Validate<T, string> validate)
      : base(loggerFactory, validate)
   {
   }

   /// <inheritdoc />
   protected override string Prepare(string key)
   {
      return key.Trim();
   }
}

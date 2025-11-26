namespace Thinktecture.EntityFrameworkCore;

/// <summary>
/// No-op max length strategy for keyed value objects that never sets a max length.
/// Use this to completely disable automatic max length configuration.
/// </summary>
public sealed class NoOpKeyedValueObjectMaxLengthStrategy
   : IKeyedValueObjectMaxLengthStrategy,
     IEquatable<NoOpKeyedValueObjectMaxLengthStrategy>
{
   /// <summary>
   /// Instance of the no-op strategy.
   /// </summary>
   public static readonly IKeyedValueObjectMaxLengthStrategy Instance = new NoOpKeyedValueObjectMaxLengthStrategy();

   /// <inheritdoc />
   public bool OverwriteExistingMaxLength => false;

   /// <inheritdoc />
   public MaxLengthChange GetMaxLength(Type type, Type keyType)
   {
      return MaxLengthChange.None;
   }

   /// <inheritdoc />
   public bool Equals(NoOpKeyedValueObjectMaxLengthStrategy? other)
   {
      return other is not null;
   }

   /// <inheritdoc />
   public override bool Equals(object? obj)
   {
      return obj is NoOpKeyedValueObjectMaxLengthStrategy;
   }

   /// <inheritdoc />
   public override int GetHashCode()
   {
      return typeof(NoOpKeyedValueObjectMaxLengthStrategy).GetHashCode();
   }
}

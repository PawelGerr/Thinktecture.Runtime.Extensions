namespace Thinktecture.EntityFrameworkCore;

/// <summary>
/// Configuration options for keyed value object EF Core integration.
/// </summary>
public sealed class KeyedValueObjectConfiguration : IEquatable<KeyedValueObjectConfiguration>
{
   /// <summary>
   /// Configuration that disables automatic max length configuration.
   /// </summary>
   public static KeyedValueObjectConfiguration NoMaxLength { get; } = new();

   /// <summary>
   /// Strategy for calculating and applying max length to keyed value object key properties.
   /// If null, no max length configuration is applied.
   /// Default is null (no automatic max length).
   /// </summary>
   public IKeyedValueObjectMaxLengthStrategy MaxLengthStrategy
   {
      get => field ?? NoOpKeyedValueObjectMaxLengthStrategy.Instance;
      init;
   }

   /// <inheritdoc />
   public bool Equals(KeyedValueObjectConfiguration? other)
   {
      if (other is null)
         return false;

      if (ReferenceEquals(this, other))
         return true;

      return MaxLengthStrategy.Equals(other.MaxLengthStrategy);
   }

   /// <inheritdoc />
   public override bool Equals(object? obj)
   {
      return obj is KeyedValueObjectConfiguration other && Equals(other);
   }

   /// <inheritdoc />
   public override int GetHashCode()
   {
      return MaxLengthStrategy.GetHashCode();
   }
}

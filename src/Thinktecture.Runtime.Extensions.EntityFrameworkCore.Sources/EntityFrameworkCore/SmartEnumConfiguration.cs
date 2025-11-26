namespace Thinktecture.EntityFrameworkCore;

/// <summary>
/// Configuration options for smart enum EF Core integration.
/// </summary>
public sealed class SmartEnumConfiguration : IEquatable<SmartEnumConfiguration>
{
   /// <summary>
   /// Default configuration with DefaultSmartEnumMaxLengthStrategy.
   /// </summary>
   public static SmartEnumConfiguration Default { get; } = new();

   /// <summary>
   /// Configuration that disables automatic max length configuration.
   /// </summary>
   public static SmartEnumConfiguration NoMaxLength { get; } = new()
                                                               {
                                                                  MaxLengthStrategy = new NoOpSmartEnumMaxLengthStrategy()
                                                               };

   /// <summary>
   /// Strategy for calculating and applying max length to smart enum key properties.
   /// If null, no max length configuration is applied.
   /// </summary>
   public ISmartEnumMaxLengthStrategy MaxLengthStrategy
   {
      get => field ?? DefaultSmartEnumMaxLengthStrategy.Instance;
      init;
   }

   /// <inheritdoc />
   public bool Equals(SmartEnumConfiguration? other)
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
      return obj is SmartEnumConfiguration other && Equals(other);
   }

   /// <inheritdoc />
   public override int GetHashCode()
   {
      return MaxLengthStrategy.GetHashCode();
   }
}

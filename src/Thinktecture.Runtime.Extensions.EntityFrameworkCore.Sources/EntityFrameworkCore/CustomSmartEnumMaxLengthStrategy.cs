namespace Thinktecture.EntityFrameworkCore;

/// <summary>
/// Custom max length strategy for smart enums that uses a user-provided function to calculate max length.
/// </summary>
public sealed class CustomSmartEnumMaxLengthStrategy
   : ISmartEnumMaxLengthStrategy,
     IEquatable<CustomSmartEnumMaxLengthStrategy>
{
   private readonly Func<Type, Type, IReadOnlyList<ISmartEnumItem>, MaxLengthChange> _calculator;

   /// <inheritdoc />
   public bool OverwriteExistingMaxLength { get; }

   /// <summary>
   /// Initializes a new instance of <see cref="CustomSmartEnumMaxLengthStrategy"/>.
   /// </summary>
   /// <param name="calculator">
   /// Function that calculates the max length based on smart enum items, enum type, and key type.
   /// </param>
   /// <param name="overwriteExistingMaxLength">
   /// Whether to overwrite existing max length configurations.
   /// </param>
   /// <exception cref="ArgumentNullException">
   /// Thrown when <paramref name="calculator"/> is null.
   /// </exception>
   public CustomSmartEnumMaxLengthStrategy(
      Func<Type, Type, IReadOnlyList<ISmartEnumItem>, MaxLengthChange> calculator,
      bool overwriteExistingMaxLength = false)
   {
      ArgumentNullException.ThrowIfNull(calculator);

      _calculator = calculator;
      OverwriteExistingMaxLength = overwriteExistingMaxLength;
   }

   /// <inheritdoc />
   public MaxLengthChange GetMaxLength(Type type, Type keyType, IReadOnlyList<ISmartEnumItem> items)
   {
      return _calculator(type, keyType, items);
   }

   /// <inheritdoc />
   public bool Equals(CustomSmartEnumMaxLengthStrategy? other)
   {
      if (other is null)
         return false;

      if (ReferenceEquals(this, other))
         return true;

      return _calculator.Equals(other._calculator)
             && OverwriteExistingMaxLength == other.OverwriteExistingMaxLength;
   }

   /// <inheritdoc />
   public override bool Equals(object? obj)
   {
      return obj is CustomSmartEnumMaxLengthStrategy other && Equals(other);
   }

   /// <inheritdoc />
   public override int GetHashCode()
   {
      return HashCode.Combine(_calculator, OverwriteExistingMaxLength);
   }
}

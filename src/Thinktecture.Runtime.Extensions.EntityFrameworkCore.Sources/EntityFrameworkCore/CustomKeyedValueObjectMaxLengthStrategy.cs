namespace Thinktecture.EntityFrameworkCore;

/// <summary>
/// Custom max length strategy for keyed value objects that uses a user-provided function.
/// </summary>
public sealed class CustomKeyedValueObjectMaxLengthStrategy
   : IKeyedValueObjectMaxLengthStrategy,
     IEquatable<CustomKeyedValueObjectMaxLengthStrategy>
{
   private readonly Func<Type, Type, MaxLengthChange> _calculator;

   /// <inheritdoc />
   public bool OverwriteExistingMaxLength { get; }

   /// <summary>
   /// Initializes a new instance of <see cref="CustomKeyedValueObjectMaxLengthStrategy"/>.
   /// </summary>
   /// <param name="calculator">
   /// Function that calculates the max length based on value object type and key type.
   /// </param>
   /// <param name="overwriteExistingMaxLength">
   /// Whether to overwrite existing max length configurations.
   /// </param>
   /// <exception cref="ArgumentNullException">
   /// Thrown when <paramref name="calculator"/> is null.
   /// </exception>
   public CustomKeyedValueObjectMaxLengthStrategy(
      Func<Type, Type, MaxLengthChange> calculator,
      bool overwriteExistingMaxLength = false)
   {
      ArgumentNullException.ThrowIfNull(calculator);

      _calculator = calculator;
      OverwriteExistingMaxLength = overwriteExistingMaxLength;
   }

   /// <inheritdoc />
   public MaxLengthChange GetMaxLength(Type type, Type keyType)
   {
      return _calculator(type, keyType);
   }

   /// <inheritdoc />
   public bool Equals(CustomKeyedValueObjectMaxLengthStrategy? other)
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
      return obj is CustomKeyedValueObjectMaxLengthStrategy other && Equals(other);
   }

   /// <inheritdoc />
   public override int GetHashCode()
   {
      return HashCode.Combine(_calculator, OverwriteExistingMaxLength);
   }
}

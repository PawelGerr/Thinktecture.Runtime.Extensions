namespace Thinktecture.EntityFrameworkCore;

/// <summary>
/// Fixed max length strategy for string-based keyed value objects that always returns a predetermined value.
/// </summary>
public sealed class FixedKeyedValueObjectMaxLengthStrategy
   : IKeyedValueObjectMaxLengthStrategy,
     IEquatable<FixedKeyedValueObjectMaxLengthStrategy>
{
   private readonly int _maxLength;

   /// <inheritdoc />
   public bool OverwriteExistingMaxLength { get; }

   /// <summary>
   /// Initializes a new instance of <see cref="FixedKeyedValueObjectMaxLengthStrategy"/>.
   /// </summary>
   /// <param name="maxLength">The fixed max length to always return.</param>
   /// <param name="overwriteExistingMaxLength">
   /// Whether to overwrite existing max length configurations.
   /// </param>
   /// <exception cref="ArgumentOutOfRangeException">
   /// Thrown when <paramref name="maxLength"/> is less than or equal to zero.
   /// </exception>
   public FixedKeyedValueObjectMaxLengthStrategy(int maxLength, bool overwriteExistingMaxLength = false)
   {
      if (maxLength <= 0)
         throw new ArgumentOutOfRangeException(nameof(maxLength), maxLength, "Max length must be greater than zero.");

      _maxLength = maxLength;
      OverwriteExistingMaxLength = overwriteExistingMaxLength;
   }

   /// <inheritdoc />
   public MaxLengthChange GetMaxLength(Type type, Type keyType)
   {
      if (keyType != typeof(string))
         return MaxLengthChange.None;

      return _maxLength;
   }

   /// <inheritdoc />
   public bool Equals(FixedKeyedValueObjectMaxLengthStrategy? other)
   {
      if (other is null)
         return false;

      if (ReferenceEquals(this, other))
         return true;

      return _maxLength == other._maxLength
             && OverwriteExistingMaxLength == other.OverwriteExistingMaxLength;
   }

   /// <inheritdoc />
   public override bool Equals(object? obj)
   {
      return
         obj is FixedKeyedValueObjectMaxLengthStrategy other && Equals(other);
   }

   /// <inheritdoc />
   public override int GetHashCode()
   {
      return HashCode.Combine(_maxLength, OverwriteExistingMaxLength);
   }
}

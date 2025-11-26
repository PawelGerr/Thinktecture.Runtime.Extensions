namespace Thinktecture.EntityFrameworkCore;

/// <summary>
/// Represents a change to the max length configuration of a property.
/// </summary>
public readonly struct MaxLengthChange
{
   /// <summary>
   /// Represents the absence of a max length change, indicating that no update to the max length configuration is required.
   /// </summary>
   public static readonly MaxLengthChange None = default;

   /// <summary>
   /// Indicates whether the max length should be changed.
   /// If false, the current max length configuration should remain unchanged.
   /// </summary>
   public bool IsSet { get; }

   /// <summary>
   /// The max length value to set. Can be null to remove max length constraint.
   /// Only valid when <see cref="IsSet"/> is true.
   /// </summary>
   public int? Value => IsSet ? field : throw new InvalidOperationException("Value is not set.");

   /// <summary>
   /// Creates a new max length change with the specified value.
   /// </summary>
   /// <param name="value">The max length value, or null to remove max length constraint.</param>
   public MaxLengthChange(int? value)
   {
      IsSet = true;
      Value = value;
   }

   /// <summary>
   /// Implicitly converts a nullable <see cref="int"/> to a <see cref="MaxLengthChange"/>.
   /// </summary>
   /// <param name="value">The max length value, or null to remove max length constraint.</param>
   public static implicit operator MaxLengthChange(int? value)
   {
      return new MaxLengthChange(value);
   }
}

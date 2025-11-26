namespace Thinktecture.EntityFrameworkCore;

/// <summary>
/// Strategy for calculating maximum length for keyed value object key properties.
/// </summary>
public interface IKeyedValueObjectMaxLengthStrategy
{
   /// <summary>
   /// Indicates whether this strategy should overwrite an existing max length
   /// configuration on the property.
   /// </summary>
   bool OverwriteExistingMaxLength { get; }

   /// <summary>
   /// Calculates the maximum length for a keyed value object property.
   /// </summary>
   /// <param name="type">The type of the keyed value object.</param>
   /// <param name="keyType">The type of the key.</param>
   /// <returns>
   /// A <see cref="MaxLengthChange"/> indicating whether to change the max length and what value to set.
   /// Return default to leave the max length unchanged.
   /// </returns>
   MaxLengthChange GetMaxLength(Type type, Type keyType);
}

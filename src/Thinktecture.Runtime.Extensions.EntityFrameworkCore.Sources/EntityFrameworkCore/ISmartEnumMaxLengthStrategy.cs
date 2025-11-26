namespace Thinktecture.EntityFrameworkCore;

/// <summary>
/// Strategy for calculating maximum length for smart enum key properties.
/// </summary>
public interface ISmartEnumMaxLengthStrategy
{
   /// <summary>
   /// Indicates whether this strategy should overwrite an existing max length
   /// configuration on the property.
   /// </summary>
   bool OverwriteExistingMaxLength { get; }

   /// <summary>
   /// Calculates the maximum length for a property based on smart enum items.
   /// </summary>
   /// <param name="type">The type of the smart enum.</param>
   /// <param name="keyType">The type of the key.</param>
   /// <param name="items">The smart enum items.</param>
   /// <returns>
   /// A <see cref="MaxLengthChange"/> indicating whether to change the max length and what value to set.
   /// Return default to leave the max length unchanged.
   /// </returns>
   MaxLengthChange GetMaxLength(Type type, Type keyType, IReadOnlyList<ISmartEnumItem> items);
}

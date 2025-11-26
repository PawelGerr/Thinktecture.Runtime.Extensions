namespace Thinktecture.EntityFrameworkCore;

/// <summary>
/// Provides information about a smart enum item for configuration strategies.
/// </summary>
public interface ISmartEnumItem
{
   /// <summary>
   /// The key value of the smart enum item.
   /// </summary>
   object Key { get; }

   /// <summary>
   /// The actual smart enum item instance.
   /// </summary>
   object Item { get; }

   /// <summary>
   /// The identifier/name of the smart enum item.
   /// </summary>
   string Identifier { get; }
}

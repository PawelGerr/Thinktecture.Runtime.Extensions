namespace Thinktecture.Swashbuckle;

/// <summary>
/// Represents a Smart Enum item with information needed for OpenAPI schema generation.
/// </summary>
public interface ISmartEnumItem
{
   /// <summary>
   /// The key of the Smart Enum item.
   /// </summary>
   object Key { get; }

   /// <summary>
   /// The Smart Enum item.
   /// </summary>
   object Item { get; }

   /// <summary>
   /// The .NET identifier of the Smart Enum item.
   /// </summary>
   string Identifier { get; }
}

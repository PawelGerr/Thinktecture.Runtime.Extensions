namespace Thinktecture.Internal;

/// <summary>
/// Contains metadata for a Smart Enum item.
/// </summary>
public sealed class SmartEnumItemMetadata
{
   /// <summary>
   /// The key of the Smart Enum item.
   /// </summary>
   public required object Key { get; init; }

   /// <summary>
   /// The Smart Enum item.
   /// </summary>
   public required object Item { get; init; }

   /// <summary>
   /// The .NET identifier of the Smart Enum item.
   /// </summary>
   public required string Identifier { get; init; }
}

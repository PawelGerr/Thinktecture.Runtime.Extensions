namespace Thinktecture.Internal;

/// <summary>
/// Contains metadata for a Smart Enum item.
/// </summary>
public sealed class KeylessSmartEnumItemMetadata
{
   /// <summary>
   /// The Smart Enum item.
   /// </summary>
   public required object Item { get; init; }

   /// <summary>
   /// The .NET identifier of the Smart Enum item.
   /// </summary>
   public required string Identifier { get; init; }
}

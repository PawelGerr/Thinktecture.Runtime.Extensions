namespace Thinktecture.EntityFrameworkCore.Internal;

internal sealed record SmartEnumItem(
   object Key,
   object Item,
   string Identifier) : ISmartEnumItem;

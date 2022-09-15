namespace Thinktecture.SmartEnums;

public sealed partial class EnumWithBaseClass : SomeBaseClass, IEnum<string>
{
   public static readonly EnumWithBaseClass Item1 = new("item 1", 42);
}

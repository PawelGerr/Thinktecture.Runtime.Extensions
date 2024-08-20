namespace Thinktecture.SmartEnums;

[SmartEnum<string>]
public partial class EnumWithBaseClass : SomeBaseClass
{
   public static readonly EnumWithBaseClass Item1 = new("item 1", 42);
}

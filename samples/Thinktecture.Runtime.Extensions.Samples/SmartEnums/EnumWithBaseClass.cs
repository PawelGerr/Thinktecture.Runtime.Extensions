namespace Thinktecture.SmartEnums;

public partial class EnumWithBaseClass : SomeBaseClass, IEnum<string>
{
   public static readonly EnumWithBaseClass Item1 = new("item 1", 42);
}

namespace Thinktecture.SmartEnums;

[SmartEnum<string>]
public partial class EnumWithBaseClass : SomeBaseClass
{
   public static readonly EnumWithBaseClass Item1 = new("item 1", "Item 1 Description", 42);

   public string Description { get; }
}

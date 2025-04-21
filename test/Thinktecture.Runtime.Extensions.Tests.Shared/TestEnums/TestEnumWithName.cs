namespace Thinktecture.Runtime.Tests.TestEnums;

[SmartEnum<int>(SkipToString = true)]
public partial class TestEnumWithName
{
   public static readonly TestEnumWithName Item1 = new(1, "item1");
   public static readonly TestEnumWithName Item2 = new(2, "item 2");

   public string Name { get; }

   public override string ToString()
   {
      return Name;
   }
}

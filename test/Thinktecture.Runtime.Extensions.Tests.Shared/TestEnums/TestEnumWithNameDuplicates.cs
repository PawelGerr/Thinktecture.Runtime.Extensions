namespace Thinktecture.Runtime.Tests.TestEnums;

[SmartEnum<int>(SkipToString = true)]
public partial class TestEnumWithNameDuplicates
{
   public static readonly TestEnumWithNameDuplicates Item1 = new(1, "item");
   public static readonly TestEnumWithNameDuplicates Item2 = new(2, "item");

   public string Name { get; }

   public override string ToString()
   {
      return Name;
   }
}

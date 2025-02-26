namespace Thinktecture.Runtime.Tests.TestEnums;

[SmartEnum<int>]
public partial class TestEnumWithDelegateGeneration
{
   public static readonly TestEnumWithDelegateGeneration Item1 = new(1, s => $"{s} + 1");
   public static readonly TestEnumWithDelegateGeneration Item2 = new(2, s => $"{s} + 2");

   [GenerateDelegate]
   public partial string Foo(int bar);
}

namespace Thinktecture.Runtime.Tests.TestEnums;

[SmartEnum<int>]
public partial class TestEnumWithDelegateGeneration
{
   public static readonly TestEnumWithDelegateGeneration Item1 = new(1, s => $"{s} + 1", s => {}, Mixed1);

   public static readonly TestEnumWithDelegateGeneration Item2 = new(2, s => $"{s} + 2", s => {}, Mixed2);

   [UseDelegateFromConstructor]
   public partial string FooFunc(int bar);

   [UseDelegateFromConstructor]
   public partial void FooAction(int bar);

   [UseDelegateFromConstructor]
   public partial string FooMixed(string normal, ref int refValue, in double inValue, out bool outValue, ref readonly decimal readonlyValue);

   private static string Mixed1(string normal, ref int value, in double inValue, out bool outValue, ref readonly decimal readonlyValue)
   {
      outValue = true;
      return "42";
   }

   private static string Mixed2(string normal, ref int value, in double inValue, out bool outValue, ref readonly decimal readonlyValue)
   {
      outValue = true;
      return "43";
   }
}

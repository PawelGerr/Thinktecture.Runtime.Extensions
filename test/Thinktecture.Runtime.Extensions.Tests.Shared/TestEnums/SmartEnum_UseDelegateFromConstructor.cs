namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
[SmartEnum<int>]
public partial class SmartEnum_UseDelegateFromConstructor
{
   public static readonly SmartEnum_UseDelegateFromConstructor Item1 = new(
      1,
      i => $"{i} + 1",
      i => { },
      s => { },
      b => { },
      Mixed1,
      tuple => default);

   public static readonly SmartEnum_UseDelegateFromConstructor Item2 = new(
      2,
      i => $"{i} + 2",
      i => { },
      s => { },
      b => { },
      Mixed2,
      tuple => default);

   [UseDelegateFromConstructor]
   public partial string FooFunc(int bar);

   [UseDelegateFromConstructor]
   public partial void FooAction(int bar);

   [UseDelegateFromConstructor(DelegateName = "StringFooAction")]
   public partial void FooAction(string? bar);

   [UseDelegateFromConstructor(DelegateName = "BoolFooAction")]
   public partial void FooAction(bool bar);

   [UseDelegateFromConstructor]
   public partial string FooMixed(string normal, ref int refValue, in double inValue, out bool outValue, ref readonly decimal readonlyValue);

   [UseDelegateFromConstructor]
   public partial (string Prop1, int Prop2) Method((bool Prop3, char Prop4) arg);

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

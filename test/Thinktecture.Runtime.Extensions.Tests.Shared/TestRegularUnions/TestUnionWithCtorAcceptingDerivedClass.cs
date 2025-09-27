namespace Thinktecture.Runtime.Tests.TestRegularUnions;

#pragma warning disable CS9113

// ReSharper disable once InconsistentNaming
[Union]
public partial class TestUnionWithCtorAcceptingDerivedClass
{
   public class Child1 : TestUnionWithCtorAcceptingDerivedClass
   {
      private Child1(Child2 value)
      {
      }

      public sealed class Child2() : Child1(null!);
   }
}

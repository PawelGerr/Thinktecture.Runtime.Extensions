namespace Thinktecture.Runtime.Tests.TestRegularUnions;

#pragma warning disable CS9113

// ReSharper disable once InconsistentNaming
[Union]
public partial class GenericTestUnionWithCtorAcceptingDerivedClass<T>
{
   public class Child1 : GenericTestUnionWithCtorAcceptingDerivedClass<T>
   {
      private Child1()
      {
      }

      public sealed class Child2(Child1 value) : Child1;
   }
}

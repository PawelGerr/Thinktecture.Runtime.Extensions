namespace Thinktecture.Runtime.Tests.TestRegularUnions;

#pragma warning disable CS9113 // Parameter is unread.

// ReSharper disable InconsistentNaming
[Union]
public partial class TestUnionWithCtorAcceptingBaseClass_1
{
   public sealed class Child1(TestUnionWithCtorAcceptingBaseClass_1 value) : TestUnionWithCtorAcceptingBaseClass_1;
}

[Union]
public partial class TestUnionWithCtorAcceptingBaseClass_2
{
   public class Child1 : TestUnionWithCtorAcceptingBaseClass_2
   {
      private Child1()
      {
      }

      public sealed class Child2(Child1 value) : Child1;
   }
}

[Union]
public partial class TestUnionWithCtorAcceptingBaseClass_3
{
   public class Child1 : TestUnionWithCtorAcceptingBaseClass_3
   {
      private Child1()
      {
      }

      public sealed class Child2(Child1 value) : Child1;
   }
}

[Union]
public partial class TestUnionWithCtorAcceptingBaseClass_4 : TestUnionWithCtorAcceptingBaseClass_TestBaseClass
{
   public sealed class Child1(TestUnionWithCtorAcceptingBaseClass_TestBaseClass value) : TestUnionWithCtorAcceptingBaseClass_4;
}

namespace Thinktecture.Runtime.Tests.TestRegularUnions;

#pragma warning disable CS9113 // Parameter is unread.

// ReSharper disable InconsistentNaming
[Union]
public partial class GenericTestUnionWithCtorAcceptingBaseClass_1<T>
{
   public sealed class Child1(TestUnionWithCtorAcceptingBaseClass_1 value) : GenericTestUnionWithCtorAcceptingBaseClass_1<T>;
}

[Union]
public partial class GenericTestUnionWithCtorAcceptingBaseClass_2<T>
{
   public class Child1 : GenericTestUnionWithCtorAcceptingBaseClass_2<T>
   {
      private Child1()
      {
      }

      public sealed class Child2(Child1 value) : Child1;
   }
}

[Union]
public partial class GenericTestUnionWithCtorAcceptingBaseClass_3<T>
{
   public class Child1 : GenericTestUnionWithCtorAcceptingBaseClass_3<T>
   {
      private Child1()
      {
      }

      public sealed class Child2(Child1 value) : Child1;
   }
}

[Union]
public partial class GenericTestUnionWithCtorAcceptingBaseClass_4 : TestUnionWithCtorAcceptingBaseClass_TestBaseClass
{
   public sealed class Child1(TestUnionWithCtorAcceptingBaseClass_TestBaseClass value) : GenericTestUnionWithCtorAcceptingBaseClass_4;
}

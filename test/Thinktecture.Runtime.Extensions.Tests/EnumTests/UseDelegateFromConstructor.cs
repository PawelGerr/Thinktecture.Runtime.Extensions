using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTests;

public class UseDelegateFromConstructor
{
   [Fact]
   public void Should_delegate_calls_to_provided_delegates()
   {
      TestEnumWithUseDelegateFromConstructor.Item1.FooFunc(42).Should().Be("42 + 1");
      TestEnumWithUseDelegateFromConstructor.Item2.FooFunc(43).Should().Be("43 + 2");
   }
}

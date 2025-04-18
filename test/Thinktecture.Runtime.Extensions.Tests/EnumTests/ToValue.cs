using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTests;

public class ToValue
{
   [Fact]
   public void Should_return_value_using_factory_specified_via_ObjectFactoryAttribute()
   {
      ((IConvertible<string>)EnumWithFactory.Item1).ToValue()
                                                              .Should().Be("=1=");

      ((IConvertible<int>)EnumWithFactory.Item1).ToValue()
                                                           .Should().Be(1);
   }
}

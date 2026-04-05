using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTests;

public class ToValue
{
   [Fact]
   public void Should_return_value_using_factory_specified_via_ObjectFactoryAttribute()
   {
      ((IConvertible<string>)SmartEnum_Factory.Item1).ToValue()
                                                              .Should().Be("=1=");

      ((IConvertible<int>)SmartEnum_Factory.Item1).ToValue()
                                                           .Should().Be(1);
   }

   [Fact]
   public void Should_return_key_for_generic_key_based_unconstraint_enum()
   {
      ((IConvertible<int>)SmartEnum_GenericKeyBasedUnconstraint<int>.Item1).ToValue()
                                                                          .Should().Be(1);
   }
}

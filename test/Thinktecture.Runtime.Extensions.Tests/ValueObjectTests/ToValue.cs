using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.ValueObjectTests;

public class ToValue
{
   [Fact]
   public void Should_return_value_using_factory_specified_via_ObjectFactoryAttribute()
   {
      BoundaryWithFactories.Create(1, 2)
                           .ToValue()
                           .Should().Be("1:2");
   }
}

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

   [Fact]
   public void Should_return_value_for_generic_key_based_reference_value_object_unconstraint()
   {
      var obj = GenericKeyBasedReferenceValueObjectUnconstraint<int>.Create(42);

      ((IConvertible<int>)obj).ToValue().Should().Be(42);
   }

   [Fact]
   public void Should_return_value_for_generic_key_based_struct_value_object()
   {
      var obj = GenericKeyBasedStructValueObject<int>.Create(42);

      ((IConvertible<int>)obj).ToValue().Should().Be(42);
   }
}

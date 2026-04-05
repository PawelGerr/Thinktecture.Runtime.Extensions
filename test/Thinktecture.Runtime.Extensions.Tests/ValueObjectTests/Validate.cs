using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.ValueObjectTests;

public class Validate
{
   [Fact]
   public void Should_create_BoundaryWithFactories_from_string()
   {
      BoundaryWithFactories.Validate("1:2", null, out var boundary)
                           .Should().BeNull();

      boundary.Should().BeEquivalentTo(BoundaryWithFactories.Create(1, 2));
   }

   [Fact]
   public void Should_create_BoundaryWithFactories_from_tuple()
   {
      BoundaryWithFactories.Validate((1, 2), null, out var boundary)
                           .Should().BeNull();

      boundary.Should().BeEquivalentTo(BoundaryWithFactories.Create(1, 2));
   }

   [Fact]
   public void Should_return_error_on_creation_of_BoundaryWithFactories_with_invalid_parameter()
   {
      BoundaryWithFactories.Validate("1", null, out var boundary)
                           .Should().BeEquivalentTo(new ValidationError("Invalid format."));

      boundary.Should().BeNull();
   }

   [Fact]
   public void Should_validate_generic_key_based_reference_value_object_unconstraint()
   {
      GenericKeyBasedReferenceValueObjectUnconstraint<int>.Validate(42, null, out var obj)
                                                          .Should().BeNull();

      obj.Should().NotBeNull();
      ((int?)obj).Should().Be(42);
   }

   [Fact]
   public void Should_validate_generic_key_based_struct_value_object()
   {
      GenericKeyBasedStructValueObject<int>.Validate(42, null, out var obj)
                                           .Should().BeNull();

      ((int?)obj).Should().Be(42);
   }
}

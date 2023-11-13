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
}

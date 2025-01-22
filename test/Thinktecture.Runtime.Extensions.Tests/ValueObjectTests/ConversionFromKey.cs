using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.ValueObjectTests;

public class ConversionFromKey
{
   [Fact]
   public void Should_return_value_object_for_key()
   {
      var value = (IntBasedReferenceValueObject)42;
      value.Should().Be(IntBasedReferenceValueObject.Create(42));
   }
}

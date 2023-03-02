using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.ValueObjectTests;

public class SubtractionOperators
{
   [Fact]
   public void Should_subtract()
   {
      var obj = DecimalBasedClassValueObject.Create(3);
      var other = DecimalBasedClassValueObject.Create(1);

      DecimalBasedClassValueObject result = obj - other;

      result.Should().Be(DecimalBasedClassValueObject.Create(2));
   }
}

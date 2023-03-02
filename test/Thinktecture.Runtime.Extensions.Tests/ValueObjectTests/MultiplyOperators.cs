using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.ValueObjectTests;

public class MultiplyOperators
{
   [Fact]
   public void Should_multiply()
   {
      var obj = DecimalBasedClassValueObject.Create(3);
      var other = DecimalBasedClassValueObject.Create(2);

      DecimalBasedClassValueObject result = obj * other;

      result.Should().Be(DecimalBasedClassValueObject.Create(6));
   }
}

using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.ValueObjectTests;

public class DivisionOperators
{
   [Fact]
   public void Should_divide()
   {
      var obj = DecimalBasedClassValueObject.Create(6);
      var other = DecimalBasedClassValueObject.Create(2);

      DecimalBasedClassValueObject result = obj / other;

      result.Should().Be(DecimalBasedClassValueObject.Create(3));
   }
}

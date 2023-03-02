using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.ValueObjectTests;

public class AdditionOperators
{
   [Fact]
   public void Should_add()
   {
      var obj = DecimalBasedClassValueObject.Create(1);
      var other = DecimalBasedClassValueObject.Create(2);

      DecimalBasedClassValueObject result = obj + other;

      result.Should().Be(DecimalBasedClassValueObject.Create(3));
   }
}

using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.ValueObjectTests;

public class AdditionOperators
{
   [Fact]
   public void Should_add_value_objects()
   {
      var obj = DecimalBasedClassValueObject.Create(1);
      var other = DecimalBasedClassValueObject.Create(2);

      DecimalBasedClassValueObject result = obj + other;

      result.Should().Be(DecimalBasedClassValueObject.Create(3));
   }

   [Fact]
   public void Should_add_value_object_with_key_type()
   {
      var obj = DecimalBasedClassValueObject.Create(1);

      DecimalBasedClassValueObject result = obj + 2;

      result.Should().Be(DecimalBasedClassValueObject.Create(3));
   }
}

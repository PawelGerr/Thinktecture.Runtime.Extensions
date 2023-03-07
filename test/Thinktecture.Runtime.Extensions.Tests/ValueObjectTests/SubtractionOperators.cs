using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.ValueObjectTests;

public class SubtractionOperators
{
   [Fact]
   public void Should_subtract_value_objects()
   {
      var obj = DecimalBasedClassValueObject.Create(3);
      var other = DecimalBasedClassValueObject.Create(1);

      DecimalBasedClassValueObject result = obj - other;

      result.Should().Be(DecimalBasedClassValueObject.Create(2));
   }

   [Fact]
   public void Should_subtract_value_object_with_key_type()
   {
      var obj = DecimalBasedClassValueObject.Create(3);

      DecimalBasedClassValueObject result = obj - 1;

      result.Should().Be(DecimalBasedClassValueObject.Create(2));
   }
}

using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.ValueObjectTests;

public class MultiplyOperators
{
   [Fact]
   public void Should_multiply_value_objects()
   {
      var obj = DecimalBasedClassValueObject.Create(3);
      var other = DecimalBasedClassValueObject.Create(2);

      DecimalBasedClassValueObject result = obj * other;

      result.Should().Be(DecimalBasedClassValueObject.Create(6));
   }

   [Fact]
   public void Should_multiply_value_object_with_key_type()
   {
      var obj = DecimalBasedClassValueObject.Create(3);

      DecimalBasedClassValueObject result = obj * 2;

      result.Should().Be(DecimalBasedClassValueObject.Create(6));
   }

   [Fact]
   public void Should_multiply_value_objects_having_custom_factory_name()
   {
      var obj = IntBasedReferenceValueObjectWithCustomFactoryNames.Get(3);
      var other = IntBasedReferenceValueObjectWithCustomFactoryNames.Get(2);

      IntBasedReferenceValueObjectWithCustomFactoryNames result = obj * other;

      result.Should().Be(IntBasedReferenceValueObjectWithCustomFactoryNames.Get(6));
   }

   [Fact]
   public void Should_multiply_value_object_with_key_type_having_custom_factory_name()
   {
      var obj = IntBasedReferenceValueObjectWithCustomFactoryNames.Get(3);

      IntBasedReferenceValueObjectWithCustomFactoryNames result = obj * 2;

      result.Should().Be(IntBasedReferenceValueObjectWithCustomFactoryNames.Get(6));
   }
}

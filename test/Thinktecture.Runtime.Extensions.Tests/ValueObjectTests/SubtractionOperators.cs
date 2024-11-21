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

   [Fact]
   public void Should_subtract_value_objects_having_custom_factory_name()
   {
      var obj = IntBasedReferenceValueObjectWithCustomFactoryNames.Get(3);
      var other = IntBasedReferenceValueObjectWithCustomFactoryNames.Get(1);

      IntBasedReferenceValueObjectWithCustomFactoryNames result = obj - other;

      result.Should().Be(IntBasedReferenceValueObjectWithCustomFactoryNames.Get(2));
   }

   [Fact]
   public void Should_subtract_value_object_with_key_type_having_custom_factory_name()
   {
      var obj = IntBasedReferenceValueObjectWithCustomFactoryNames.Get(3);

      IntBasedReferenceValueObjectWithCustomFactoryNames result = obj - 1;

      result.Should().Be(IntBasedReferenceValueObjectWithCustomFactoryNames.Get(2));
   }
}

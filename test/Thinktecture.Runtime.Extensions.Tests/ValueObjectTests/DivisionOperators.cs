using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.ValueObjectTests;

public class DivisionOperators
{
   [Fact]
   public void Should_divide_value_objects()
   {
      var obj = DecimalBasedClassValueObject.Create(6);
      var other = DecimalBasedClassValueObject.Create(2);

      DecimalBasedClassValueObject result = obj / other;

      result.Should().Be(DecimalBasedClassValueObject.Create(3));
   }

   [Fact]
   public void Should_divide_value_object_with_key_type()
   {
      var obj = DecimalBasedClassValueObject.Create(6);

      DecimalBasedClassValueObject result = obj / 2;

      result.Should().Be(DecimalBasedClassValueObject.Create(3));
   }

   [Fact]
   public void Should_divide_value_objects_having_custom_factory_name()
   {
      var obj = IntBasedReferenceValueObjectWithCustomFactoryNames.Get(6);
      var other = IntBasedReferenceValueObjectWithCustomFactoryNames.Get(2);

      IntBasedReferenceValueObjectWithCustomFactoryNames result = obj / other;

      result.Should().Be(IntBasedReferenceValueObjectWithCustomFactoryNames.Get(3));
   }

   [Fact]
   public void Should_divide_value_object_with_key_type_having_custom_factory_name()
   {
      var obj = IntBasedReferenceValueObjectWithCustomFactoryNames.Get(6);

      IntBasedReferenceValueObjectWithCustomFactoryNames result = obj / 2;

      result.Should().Be(IntBasedReferenceValueObjectWithCustomFactoryNames.Get(3));
   }
}

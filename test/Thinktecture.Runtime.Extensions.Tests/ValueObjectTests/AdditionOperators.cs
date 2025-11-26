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
   public void Should_add_value_objects_having_custom_factory_name()
   {
      var obj = IntBasedReferenceValueObjectWithCustomFactoryNames.Get(1);
      var other = IntBasedReferenceValueObjectWithCustomFactoryNames.Get(2);

      IntBasedReferenceValueObjectWithCustomFactoryNames result = obj + other;

      result.Should().Be(IntBasedReferenceValueObjectWithCustomFactoryNames.Get(3));
   }

   [Fact]
   public void Should_add_value_object_with_key_type()
   {
      var obj = DecimalBasedClassValueObject.Create(1);

      DecimalBasedClassValueObject result = obj + 2;

      result.Should().Be(DecimalBasedClassValueObject.Create(3));
   }

   [Fact]
   public void Should_add_value_object_with_key_type_having_custom_factory_name()
   {
      var obj = IntBasedReferenceValueObjectWithCustomFactoryNames.Get(1);

      IntBasedReferenceValueObjectWithCustomFactoryNames result = obj + 2;

      result.Should().Be(IntBasedReferenceValueObjectWithCustomFactoryNames.Get(3));
   }

   [Fact]
   public void Should_wrap_around_on_overflow_when_adding_int_max_value()
   {
      var obj = IntBasedReferenceValueObjectWithCustomFactoryNames.Get(int.MaxValue);
      var other = IntBasedReferenceValueObjectWithCustomFactoryNames.Get(1);

      IntBasedReferenceValueObjectWithCustomFactoryNames result = obj + other;

      result.Should().Be(IntBasedReferenceValueObjectWithCustomFactoryNames.Get(int.MinValue));
   }

   [Fact]
   public void Should_wrap_around_on_overflow_when_adding_int_max_value_with_key_type()
   {
      var obj = IntBasedReferenceValueObjectWithCustomFactoryNames.Get(int.MaxValue);

      IntBasedReferenceValueObjectWithCustomFactoryNames result = obj + 1;

      result.Should().Be(IntBasedReferenceValueObjectWithCustomFactoryNames.Get(int.MinValue));
   }

   [Fact]
   public void Should_add_negative_values_correctly()
   {
      var obj = IntBasedReferenceValueObjectWithCustomFactoryNames.Get(-5);
      var other = IntBasedReferenceValueObjectWithCustomFactoryNames.Get(3);

      IntBasedReferenceValueObjectWithCustomFactoryNames result = obj + other;

      result.Should().Be(IntBasedReferenceValueObjectWithCustomFactoryNames.Get(-2));
   }

   [Fact]
   public void Should_add_zero_to_value_object()
   {
      var obj = IntBasedReferenceValueObjectWithCustomFactoryNames.Get(10);
      var zero = IntBasedReferenceValueObjectWithCustomFactoryNames.Get(0);

      IntBasedReferenceValueObjectWithCustomFactoryNames result = obj + zero;

      result.Should().Be(obj);
   }

   [Fact]
   public void Should_add_decimal_values_without_overflow()
   {
      var obj = DecimalBasedClassValueObject.Create(decimal.MaxValue - 1);
      var other = DecimalBasedClassValueObject.Create(1);

      DecimalBasedClassValueObject result = obj + other;

      result.Should().Be(DecimalBasedClassValueObject.Create(decimal.MaxValue));
   }
}

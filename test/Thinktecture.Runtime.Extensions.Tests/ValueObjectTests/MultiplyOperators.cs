using System;
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

   [Fact]
   public void Should_wrap_around_on_overflow_when_multiplying_int_max_value()
   {
      var obj = IntBasedReferenceValueObjectWithCustomFactoryNames.Get(int.MaxValue);
      var other = IntBasedReferenceValueObjectWithCustomFactoryNames.Get(2);

      IntBasedReferenceValueObjectWithCustomFactoryNames result = obj * other;

      // unchecked: int.MaxValue * 2 wraps around
      result.Property.Should().Be(unchecked(int.MaxValue * 2));
   }

   [Fact]
   public void Should_wrap_around_on_overflow_when_multiplying_int_max_value_with_key_type()
   {
      var obj = IntBasedReferenceValueObjectWithCustomFactoryNames.Get(int.MaxValue);

      IntBasedReferenceValueObjectWithCustomFactoryNames result = obj * 2;

      // unchecked: int.MaxValue * 2 wraps around
      result.Property.Should().Be(unchecked(int.MaxValue * 2));
   }

   [Fact]
   public void Should_wrap_around_on_overflow_when_multiplying_large_values()
   {
      var obj = IntBasedReferenceValueObjectWithCustomFactoryNames.Get(1000000);
      var other = IntBasedReferenceValueObjectWithCustomFactoryNames.Get(10000);

      IntBasedReferenceValueObjectWithCustomFactoryNames result = obj * other;

      // unchecked: 1000000 * 10000 wraps around
      result.Property.Should().Be(unchecked(1000000 * 10000));
   }

   [Fact]
   public void Should_multiply_by_zero_correctly()
   {
      var obj = IntBasedReferenceValueObjectWithCustomFactoryNames.Get(10);
      var zero = IntBasedReferenceValueObjectWithCustomFactoryNames.Get(0);

      IntBasedReferenceValueObjectWithCustomFactoryNames result = obj * zero;

      result.Should().Be(IntBasedReferenceValueObjectWithCustomFactoryNames.Get(0));
   }

   [Fact]
   public void Should_multiply_by_one_correctly()
   {
      var obj = IntBasedReferenceValueObjectWithCustomFactoryNames.Get(10);
      var one = IntBasedReferenceValueObjectWithCustomFactoryNames.Get(1);

      IntBasedReferenceValueObjectWithCustomFactoryNames result = obj * one;

      result.Should().Be(obj);
   }

   [Fact]
   public void Should_multiply_negative_values_correctly()
   {
      var obj = IntBasedReferenceValueObjectWithCustomFactoryNames.Get(-5);
      var other = IntBasedReferenceValueObjectWithCustomFactoryNames.Get(3);

      IntBasedReferenceValueObjectWithCustomFactoryNames result = obj * other;

      result.Should().Be(IntBasedReferenceValueObjectWithCustomFactoryNames.Get(-15));
   }

   [Fact]
   public void Should_multiply_two_negative_values_to_positive()
   {
      var obj = IntBasedReferenceValueObjectWithCustomFactoryNames.Get(-5);
      var other = IntBasedReferenceValueObjectWithCustomFactoryNames.Get(-3);

      IntBasedReferenceValueObjectWithCustomFactoryNames result = obj * other;

      result.Should().Be(IntBasedReferenceValueObjectWithCustomFactoryNames.Get(15));
   }
}

using System;
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

   [Fact]
   public void Should_wrap_around_on_underflow_when_subtracting_from_int_min_value()
   {
      var obj = IntBasedReferenceValueObjectWithCustomFactoryNames.Get(int.MinValue);
      var other = IntBasedReferenceValueObjectWithCustomFactoryNames.Get(1);

      IntBasedReferenceValueObjectWithCustomFactoryNames result = obj - other;

      result.Should().Be(IntBasedReferenceValueObjectWithCustomFactoryNames.Get(int.MaxValue));
   }

   [Fact]
   public void Should_wrap_around_on_underflow_when_subtracting_from_int_min_value_with_key_type()
   {
      var obj = IntBasedReferenceValueObjectWithCustomFactoryNames.Get(int.MinValue);

      IntBasedReferenceValueObjectWithCustomFactoryNames result = obj - 1;

      result.Should().Be(IntBasedReferenceValueObjectWithCustomFactoryNames.Get(int.MaxValue));
   }

   [Fact]
   public void Should_subtract_negative_values_correctly()
   {
      var obj = IntBasedReferenceValueObjectWithCustomFactoryNames.Get(5);
      var other = IntBasedReferenceValueObjectWithCustomFactoryNames.Get(-3);

      IntBasedReferenceValueObjectWithCustomFactoryNames result = obj - other;

      result.Should().Be(IntBasedReferenceValueObjectWithCustomFactoryNames.Get(8));
   }

   [Fact]
   public void Should_subtract_zero_from_value_object()
   {
      var obj = IntBasedReferenceValueObjectWithCustomFactoryNames.Get(10);
      var zero = IntBasedReferenceValueObjectWithCustomFactoryNames.Get(0);

      IntBasedReferenceValueObjectWithCustomFactoryNames result = obj - zero;

      result.Should().Be(obj);
   }

   [Fact]
   public void Should_result_in_negative_when_subtracting_larger_from_smaller()
   {
      var obj = IntBasedReferenceValueObjectWithCustomFactoryNames.Get(3);
      var other = IntBasedReferenceValueObjectWithCustomFactoryNames.Get(5);

      IntBasedReferenceValueObjectWithCustomFactoryNames result = obj - other;

      result.Should().Be(IntBasedReferenceValueObjectWithCustomFactoryNames.Get(-2));
   }

   [Fact]
   public void Should_subtract_decimal_values_without_underflow()
   {
      var obj = DecimalBasedClassValueObject.Create(decimal.MinValue + 1);
      var other = DecimalBasedClassValueObject.Create(1);

      DecimalBasedClassValueObject result = obj - other;

      result.Should().Be(DecimalBasedClassValueObject.Create(decimal.MinValue));
   }
}

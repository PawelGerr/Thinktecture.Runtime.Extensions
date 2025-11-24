using System;
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

   [Fact]
   public void Should_throw_DivideByZeroException_when_dividing_by_zero_value_object()
   {
      var obj = IntBasedReferenceValueObjectWithCustomFactoryNames.Get(10);
      var zero = IntBasedReferenceValueObjectWithCustomFactoryNames.Get(0);

      FluentActions.Invoking(() =>
      {
         var result = obj / zero;
      }).Should().Throw<DivideByZeroException>();
   }

   [Fact]
   public void Should_throw_DivideByZeroException_when_dividing_by_zero_key_type()
   {
      var obj = IntBasedReferenceValueObjectWithCustomFactoryNames.Get(10);

      FluentActions.Invoking(() =>
      {
         var result = obj / 0;
      }).Should().Throw<DivideByZeroException>();
   }

   [Fact]
   public void Should_divide_by_one_correctly()
   {
      var obj = IntBasedReferenceValueObjectWithCustomFactoryNames.Get(10);
      var one = IntBasedReferenceValueObjectWithCustomFactoryNames.Get(1);

      IntBasedReferenceValueObjectWithCustomFactoryNames result = obj / one;

      result.Should().Be(obj);
   }

   [Fact]
   public void Should_divide_zero_by_value_object()
   {
      var zero = IntBasedReferenceValueObjectWithCustomFactoryNames.Get(0);
      var obj = IntBasedReferenceValueObjectWithCustomFactoryNames.Get(10);

      IntBasedReferenceValueObjectWithCustomFactoryNames result = zero / obj;

      result.Should().Be(IntBasedReferenceValueObjectWithCustomFactoryNames.Get(0));
   }

   [Fact]
   public void Should_divide_negative_values_correctly()
   {
      var obj = IntBasedReferenceValueObjectWithCustomFactoryNames.Get(-10);
      var other = IntBasedReferenceValueObjectWithCustomFactoryNames.Get(2);

      IntBasedReferenceValueObjectWithCustomFactoryNames result = obj / other;

      result.Should().Be(IntBasedReferenceValueObjectWithCustomFactoryNames.Get(-5));
   }

   [Fact]
   public void Should_divide_two_negative_values_to_positive()
   {
      var obj = IntBasedReferenceValueObjectWithCustomFactoryNames.Get(-10);
      var other = IntBasedReferenceValueObjectWithCustomFactoryNames.Get(-2);

      IntBasedReferenceValueObjectWithCustomFactoryNames result = obj / other;

      result.Should().Be(IntBasedReferenceValueObjectWithCustomFactoryNames.Get(5));
   }

   [Fact]
   public void Should_perform_integer_division_truncating_remainder()
   {
      var obj = IntBasedReferenceValueObjectWithCustomFactoryNames.Get(7);
      var other = IntBasedReferenceValueObjectWithCustomFactoryNames.Get(2);

      IntBasedReferenceValueObjectWithCustomFactoryNames result = obj / other;

      result.Should().Be(IntBasedReferenceValueObjectWithCustomFactoryNames.Get(3));
   }

   [Fact]
   public void Should_divide_decimal_values_preserving_precision()
   {
      var obj = DecimalBasedClassValueObject.Create(7.5m);
      var other = DecimalBasedClassValueObject.Create(2.5m);

      DecimalBasedClassValueObject result = obj / other;

      result.Should().Be(DecimalBasedClassValueObject.Create(3.0m));
   }

   [Fact]
   public void Should_throw_DivideByZeroException_when_dividing_decimal_by_zero()
   {
      var obj = DecimalBasedClassValueObject.Create(10);
      var zero = DecimalBasedClassValueObject.Create(0);

      FluentActions.Invoking(() =>
      {
         var result = obj / zero;
      }).Should().Throw<DivideByZeroException>();
   }
}

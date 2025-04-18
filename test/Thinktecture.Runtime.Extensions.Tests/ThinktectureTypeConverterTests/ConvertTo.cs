using System;
using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.ThinktectureTypeConverterTests;

public class ConvertTo : TypeConverterTestsBase
{
   [Fact]
   public void Should_return_key_if_value_type_is_reference_type_and_key_type_matches_the_struct_key()
   {
      IntBasedReferenceValueObjectTypeConverter.ConvertTo(IntBasedReferenceValueObject.Create(42), typeof(int)).Should().Be(42);
   }

   [Fact]
   public void Should_return_key_if_value_type_is_reference_type_and_key_type_matches_the_nullable_struct_key()
   {
      IntBasedReferenceValueObjectTypeConverter.ConvertTo(IntBasedReferenceValueObject.Create(42), typeof(int?)).Should().Be(42);
   }

   [Fact]
   public void Should_return_null_if_value_type_is_null_and_key_type_matches_the_nullable_struct_key()
   {
      IntBasedReferenceValueObjectTypeConverter.ConvertTo(null!, typeof(int?)).Should().BeNull();
   }

   [Fact]
   public void Should_throw_if_value_type_is_reference_type_and_null_and_key_type_matches_the_struct_key()
   {
      IntBasedReferenceValueObjectTypeConverter.Invoking(c => c.ConvertTo(null!, typeof(int)))
                                               .Should().Throw<NotSupportedException>()
                                               .WithMessage("Int32 is a struct and cannot be converted to 'null'.");
   }

   [Fact]
   public void Should_return_key_if_value_type_is_struct_and_key_matches_the_struct_key()
   {
      IntBasedStructValueObjectTypeConverter.ConvertTo(IntBasedStructValueObject.Create(42), typeof(int)).Should().Be(42);
   }

   [Fact]
   public void Should_throw_if_value_type_is_struct_and_null_and_key_type_matches_the_struct_key()
   {
      IntBasedStructValueObjectTypeConverter.Invoking(c => c.ConvertTo(null!, typeof(int)))
                                            .Should().Throw<NotSupportedException>()
                                            .WithMessage("Int32 is a struct and cannot be converted to 'null'.");
   }

   [Fact]
   public void Should_return_self_if_converted_to_itself()
   {
      var valueObject = IntBasedStructValueObject.Create(42);
      IntBasedStructValueObjectTypeConverter.ConvertTo(valueObject, typeof(IntBasedStructValueObject)).Should().Be(valueObject);
   }

   [Fact]
   public void Should_return_valid_instance_if_converted_to_nullable_type_of_itself()
   {
      IntBasedStructValueObject? valueObject = IntBasedStructValueObject.Create(42);
      IntBasedStructValueObjectTypeConverter.ConvertTo(valueObject, typeof(IntBasedStructValueObject?)).Should().Be(valueObject.Value);
   }

   [Fact]
   public void Should_return_null_if_value_type_is_struct_and_null_and_converted_to_nullable_type_of_itself()
   {
      IntBasedStructValueObjectTypeConverter.ConvertTo(null!, typeof(IntBasedStructValueObject?)).Should().BeNull();
   }

   [Fact]
   public void Should_return_key_if_value_type_is_reference_type_and_key_matches_the_reference_type_key()
   {
      StringBasedReferenceValueObjectTypeConverter.ConvertTo(StringBasedReferenceValueObject.Create("value"), typeof(string)).Should().Be("value");
   }

   [Fact]
   public void Should_return_null_if_value_type_is_reference_type_and_null_and_key_matches_the_reference_type_key()
   {
      StringBasedReferenceValueObjectTypeConverter.ConvertTo(null!, typeof(string)).Should().BeNull();
   }

   [Fact]
   public void Should_return_true_if_value_type_is_struct_and_key_type_matches_the_reference_type_key()
   {
      StringBasedStructValueObjectTypeConverter.ConvertTo(StringBasedStructValueObject.Create("value"), typeof(string)).Should().Be("value");
   }

   [Fact]
   public void Should_return_null_if_value_type_is_struct_and_null_and_key_matches_the_reference_type_key()
   {
      StringBasedStructValueObjectTypeConverter.ConvertTo(null!, typeof(string)).Should().BeNull();
   }
}

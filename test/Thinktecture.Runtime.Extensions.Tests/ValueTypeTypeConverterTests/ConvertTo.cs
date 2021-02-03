using System;
using FluentAssertions;
using Thinktecture.Runtime.Tests.TestValueTypes;
using Xunit;

namespace Thinktecture.Runtime.Tests.ValueTypeTypeConverterTests
{
   public class ConvertTo : TypeConverterTestsBase
   {
      [Fact]
      public void Should_return_key_if_value_type_is_reference_type_and_key_type_matches_the_struct_key()
      {
         IntBasedReferenceValueTypeConverter.ConvertTo(IntBasedReferenceValueType.Create(42), typeof(int)).Should().Be(42);
      }

      [Fact]
      public void Should_return_key_if_value_type_is_reference_type_and_key_type_matches_the_nullable_struct_key()
      {
         IntBasedReferenceValueTypeConverter.ConvertTo(IntBasedReferenceValueType.Create(42), typeof(int?)).Should().Be(42);
      }

      [Fact]
      public void Should_return_null_if_value_type_is_null_and_key_type_matches_the_nullable_struct_key()
      {
         IntBasedReferenceValueTypeConverter.ConvertTo(null!, typeof(int?)).Should().BeNull();
      }

      [Fact]
      public void Should_throw_if_value_type_is_reference_type_and_null_and_key_type_matches_the_struct_key()
      {
         IntBasedReferenceValueTypeConverter.Invoking(c => c.ConvertTo(null!, typeof(int)))
                                            .Should().Throw<NotSupportedException>()
                                            .WithMessage("Int32 is a struct and cannot be converted to 'null'.");
      }

      [Fact]
      public void Should_return_key_if_value_type_is_struct_and_key_matches_the_struct_key()
      {
         IntBasedStructValueTypeConverter.ConvertTo(IntBasedStructValueType.Create(42), typeof(int)).Should().Be(42);
      }

      [Fact]
      public void Should_throw_if_value_type_is_struct_and_null_and_key_type_matches_the_struct_key()
      {
         IntBasedStructValueTypeConverter.Invoking(c => c.ConvertTo(null!, typeof(int)))
                                         .Should().Throw<NotSupportedException>()
                                         .WithMessage("Int32 is a struct and cannot be converted to 'null'.");
      }

      [Fact]
      public void Should_return_self_if_converted_to_itself()
      {
         var valueType = IntBasedStructValueType.Create(42);
         IntBasedStructValueTypeConverter.ConvertTo(valueType, typeof(IntBasedStructValueType)).Should().Be(valueType);
      }

      [Fact]
      public void Should_return_valid_instance_if_converted_to_nullable_type_of_itself()
      {
         IntBasedStructValueType? valueType = IntBasedStructValueType.Create(42);
         IntBasedStructValueTypeConverter.ConvertTo(valueType, typeof(IntBasedStructValueType?)).Should().Be(valueType.Value);
      }

      [Fact]
      public void Should_return_null_if_value_type_is_struct_and_null_and_converted_to_nullable_type_of_itself()
      {
         IntBasedStructValueTypeConverter.ConvertTo(null!, typeof(IntBasedStructValueType?)).Should().BeNull();
      }

      [Fact]
      public void Should_return_key_if_value_type_is_reference_type_and_key_matches_the_reference_type_key()
      {
         StringBasedReferenceValueTypeConverter.ConvertTo(StringBasedReferenceValueType.Create("value"), typeof(string)).Should().Be("value");
      }

      [Fact]
      public void Should_return_null_if_value_type_is_reference_type_and_null_and_key_matches_the_reference_type_key()
      {
         StringBasedReferenceValueTypeConverter.ConvertTo(null!, typeof(string)).Should().BeNull();
      }

      [Fact]
      public void Should_return_true_if_value_type_is_struct_and_key_type_matches_the_reference_type_key()
      {
         StringBasedStructValueTypeConverter.ConvertTo(StringBasedStructValueType.Create("value"), typeof(string)).Should().Be("value");
      }

      [Fact]
      public void Should_return_null_if_value_type_is_struct_and_null_and_key_matches_the_reference_type_key()
      {
         StringBasedStructValueTypeConverter.ConvertTo(null!, typeof(string)).Should().BeNull();
      }
   }
}

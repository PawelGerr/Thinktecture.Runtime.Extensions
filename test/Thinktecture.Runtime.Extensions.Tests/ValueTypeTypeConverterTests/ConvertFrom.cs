using System;
using FluentAssertions;
using Thinktecture.TestValueTypes;
using Xunit;

namespace Thinktecture.ValueTypeTypeConverterTests
{
   public class ConvertFrom : TypeConverterTestsBase
   {
      [Fact]
      public void Should_return_valid_instance_if_value_type_is_reference_type_and_key_matches_the_struct_key()
      {
         IntBasedReferenceValueTypeConverter.ConvertFrom(42).Should().BeEquivalentTo(IntBasedReferenceValueType.Create(42));
      }

      [Fact]
      public void Should_return_valid_instance_if_value_type_is_struct_and_key_matches_the_struct_key()
      {
         IntBasedStructValueTypeConverter.ConvertFrom(42).Should().BeEquivalentTo(IntBasedStructValueType.Create(42));
      }

      [Fact]
      public void Should_return_self_if_converted_from_itself()
      {
         var valueType = IntBasedStructValueType.Create(42);
         IntBasedStructValueTypeConverter.ConvertFrom(valueType).Should().Be(valueType);
      }

      [Fact]
      public void Should_return_valid_instance_if_converted_from_nullable_type_of_itself()
      {
         IntBasedStructValueType? valueType = IntBasedStructValueType.Create(42);
         IntBasedStructValueTypeConverter.ConvertFrom(valueType).Should().Be(valueType.Value);
      }

      [Fact]
      public void Should_return_null_if_value_type_is_reference_type_and_key_is_null()
      {
         IntBasedReferenceValueTypeConverter.ConvertFrom(null).Should().BeNull();
      }

      [Fact]
      public void Should_throw_if_value_type_is_struct_and_key_is_null()
      {
         IntBasedStructValueTypeConverter.Invoking(c => c.ConvertFrom(null))
                                         .Should().Throw<NotSupportedException>()
                                         .WithMessage("IntBasedStructValueType is a struct and cannot be converted from 'null'.");
      }

      [Fact]
      public void Should_return_valid_instance_if_value_type_is_reference_type_and_key_matches_the_reference_type_key()
      {
         StringBasedReferenceValueTypeConverter.ConvertFrom("value").Should().BeEquivalentTo(StringBasedReferenceValueType.Create("value"));
      }

      [Fact]
      public void Should_return_null_if_value_type_is_reference_type_and_key_matches_the_reference_type_key()
      {
         StringBasedReferenceValueTypeConverter.ConvertFrom(null).Should().BeNull();
      }

      [Fact]
      public void Should_return_true_if_value_type_is_struct_and_key_type_matches_the_reference_type_key()
      {
         StringBasedStructValueTypeConverter.ConvertFrom("value").Should().BeEquivalentTo(StringBasedStructValueType.Create("value"));
      }

      [Fact]
      public void Should_throw_if_value_type_is_struct_and_string_based_key_is_null()
      {
         StringBasedStructValueTypeConverter.Invoking(c => c.ConvertFrom(null))
                                            .Should().Throw<NotSupportedException>()
                                            .WithMessage("StringBasedStructValueType is a struct and cannot be converted from 'null'.");
      }
   }
}

using System;
using FluentAssertions;
using Thinktecture.TestValueTypes;
using Xunit;

namespace Thinktecture.ValueTypeTypeConverterTests
{
   public class CanConvertTo : TypeConverterTestsBase
   {
      [Fact]
      public void Should_return_true_if_value_type_is_reference_type_and_key_type_matches_the_struct_key()
      {
         IntBasedReferenceValueTypeConverter.CanConvertTo(typeof(int)).Should().BeTrue();
      }

      [Fact]
      public void Should_return_true_if_value_type_is_struct_and_key_type_matches_the_struct_key()
      {
         IntBasedStructValueTypeConverter.CanConvertTo(typeof(int)).Should().BeTrue();
      }

      [Fact]
      public void Should_return_true_if_destination_type_is_value_type_itself()
      {
         IntBasedStructValueTypeConverter.CanConvertTo(typeof(IntBasedStructValueType)).Should().BeTrue();
      }

      [Fact]
      public void Should_return_true_if_destination_type_is_nullable_value_type_of_itself()
      {
         IntBasedStructValueTypeConverter.CanConvertTo(typeof(IntBasedStructValueType?)).Should().BeTrue();
      }

      [Fact]
      public void Should_return_true_if_value_type_is_reference_type_and_key_type_matches_the_nullable_struct_key()
      {
         IntBasedReferenceValueTypeConverter.CanConvertTo(typeof(int?)).Should().BeTrue();
      }

      [Fact]
      public void Should_return_true_if_value_type_is_struct_and_key_type_matches_the_nullable_struct_key()
      {
         IntBasedStructValueTypeConverter.CanConvertTo(typeof(int?)).Should().BeTrue();
      }

      [Fact]
      public void Should_return_true_if_value_type_is_reference_type_and_key_type_matches_the_reference_type_key()
      {
         StringBasedReferenceValueTypeConverter.CanConvertTo(typeof(string)).Should().BeTrue();
      }

      [Fact]
      public void Should_return_true_if_value_type_is_struct_and_key_type_matches_the_reference_type_key()
      {
         StringBasedStructValueTypeConverter.CanConvertTo(typeof(string)).Should().BeTrue();
      }
   }
}

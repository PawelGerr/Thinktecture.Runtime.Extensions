using System;
using FluentAssertions;
using Thinktecture.Runtime.Tests.TestValueTypes;
using Xunit;

namespace Thinktecture.Runtime.Tests.ValueTypeTypeConverterTests
{
   public class CanConvertFrom : TypeConverterTestsBase
   {
      [Fact]
      public void Should_return_true_if_value_type_is_reference_type_and_key_type_matches_the_struct_key()
      {
         IntBasedReferenceValueTypeConverter.CanConvertFrom(typeof(int)).Should().BeTrue();
      }

      [Fact]
      public void Should_return_true_if_value_type_is_struct_and_key_type_matches_the_struct_key()
      {
         IntBasedStructValueTypeConverter.CanConvertFrom(typeof(int)).Should().BeTrue();
      }

      [Fact]
      public void Should_return_true_if_source_type_is_value_type_itself()
      {
         IntBasedStructValueTypeConverter.CanConvertFrom(typeof(IntBasedStructValueType)).Should().BeTrue();
      }

      [Fact]
      public void Should_return_true_if_source_type_is_nullable_value_type_of_itself()
      {
         IntBasedStructValueTypeConverter.CanConvertFrom(typeof(IntBasedStructValueType?)).Should().BeTrue();
      }

      [Fact]
      public void Should_return_true_if_value_type_is_reference_type_and_key_type_matches_the_nullable_struct_key()
      {
         IntBasedReferenceValueTypeConverter.CanConvertFrom(typeof(int?)).Should().BeTrue();
      }

      [Fact]
      public void Should_return_true_if_value_type_is_struct_and_key_type_matches_the_nullable_struct_key()
      {
         IntBasedStructValueTypeConverter.CanConvertFrom(typeof(int?)).Should().BeTrue();
      }

      [Fact]
      public void Should_return_true_if_value_type_is_reference_type_and_key_type_matches_the_reference_type_key()
      {
         StringBasedReferenceValueTypeConverter.CanConvertFrom(typeof(string)).Should().BeTrue();
      }

      [Fact]
      public void Should_return_true_if_value_type_is_struct_and_key_type_matches_the_reference_type_key()
      {
         StringBasedStructValueTypeConverter.CanConvertFrom(typeof(string)).Should().BeTrue();
      }
   }
}

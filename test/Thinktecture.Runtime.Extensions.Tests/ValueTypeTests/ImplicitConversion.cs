using System;
using FluentAssertions;
using Thinktecture.TestValueTypes;
using Xunit;

namespace Thinktecture.ValueTypeTests
{
   public class ImplicitConversion
   {
      [Fact]
      public void Should_return_nullable_key_if_value_type_is_reference_type_and_key_is_struct()
      {
         int? value = IntBasedReferenceValueType.Create(42);
         value.Should().Be(42);
      }

      [Fact]
      public void Should_return_null_if_value_type_is_reference_type_and_null_and_key_is_struct()
      {
         IntBasedReferenceValueType obj = null;
         int? value = obj;

         value.Should().BeNull();
      }

      [Fact]
      public void Should_return_key_if_value_type_is_reference_type_and_key_is_reference_type()
      {
         string value = StringBasedReferenceValueType.Create("value");
         value.Should().Be("value");
      }

      [Fact]
      public void Should_return_null_if_value_type_is_reference_type_and_null_and_key_is_reference_type()
      {
         StringBasedReferenceValueType obj = null;
         string value = obj;
         value.Should().BeNull();
      }
   }
}

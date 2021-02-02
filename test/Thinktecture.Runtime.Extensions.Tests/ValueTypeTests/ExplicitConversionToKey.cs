using System;
using FluentAssertions;
using Thinktecture.TestValueTypes;
using Xunit;

namespace Thinktecture.ValueTypeTests
{
   public class ExplicitConversionToKey
   {
      [Fact]
      public void Should_throw_trying_convert_null_to_struct_key()
      {
         IntBasedReferenceValueType obj = null;

         obj.Invoking(o => (int)o)
            .Should().Throw<NullReferenceException>();
      }

      [Fact]
      public void Should_return_key_if_value_type_is_reference_type_and_key_is_struct()
      {
         var value = (int)IntBasedReferenceValueType.Create(42);
         value.Should().Be(42);
      }

      [Fact]
      public void Should_return_nullable_key_if_value_type_is_reference_type_and_key_is_struct()
      {
         var value = (int?)IntBasedReferenceValueType.Create(42);
         value.Should().Be(42);
      }

      [Fact]
      public void Should_return_key_if_value_type_is_struct_and_key_is_struct()
      {
         var value = (int)IntBasedStructValueType.Create(42);
         value.Should().Be(42);
      }

      [Fact]
      public void Should_return_nullable_key_if_value_type_is_struct_and_key_is_struct()
      {
         var value = (int?)IntBasedStructValueType.Create(42);
         value.Should().Be(42);
      }

      [Fact]
      public void Should_return_key_if_value_type_is_struct_and_key_is_reference_type()
      {
         var value = (string)StringBasedStructValueType.Create("value");
         value.Should().Be("value");
      }

      [Fact]
      public void Should_return_null_if_value_type_is_reference_type_and_null_and_key_is_reference_type()
      {
         StringBasedReferenceValueType obj = null;
         var value = (string)obj;

         value.Should().BeNull();
      }
   }
}

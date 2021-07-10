using System;
using FluentAssertions;
using Thinktecture.Runtime.Tests.TestValueObjects;
using Xunit;

namespace Thinktecture.Runtime.Tests.ValueObjectTypeConverterTests
{
   public class ConvertFrom : TypeConverterTestsBase
   {
      [Fact]
      public void Should_return_valid_instance_if_value_type_is_reference_type_and_key_matches_the_struct_key()
      {
         IntBasedReferenceValueObjectTypeConverter.ConvertFrom(42).Should().BeEquivalentTo(IntBasedReferenceValueObject.Create(42));
      }

      [Fact]
      public void Should_return_valid_instance_if_value_type_is_struct_and_key_matches_the_struct_key()
      {
         IntBasedStructValueObjectTypeConverter.ConvertFrom(42).Should().BeEquivalentTo(IntBasedStructValueObject.Create(42));
      }

      [Fact]
      public void Should_return_self_if_converted_from_itself()
      {
         var valueObject = IntBasedStructValueObject.Create(42);
         IntBasedStructValueObjectTypeConverter.ConvertFrom(valueObject).Should().Be(valueObject);
      }

      [Fact]
      public void Should_return_valid_instance_if_converted_from_nullable_type_of_itself()
      {
         IntBasedStructValueObject? valueObject = IntBasedStructValueObject.Create(42);
         IntBasedStructValueObjectTypeConverter.ConvertFrom(valueObject).Should().Be(valueObject.Value);
      }

      [Fact]
      public void Should_return_null_if_value_type_is_reference_type_and_key_is_null()
      {
         IntBasedReferenceValueObjectTypeConverter.ConvertFrom(null).Should().BeNull();
      }

      [Fact]
      public void Should_throw_if_value_type_is_struct_and_key_is_null()
      {
         IntBasedStructValueObjectTypeConverter.Invoking(c => c.ConvertFrom(null))
                                               .Should().Throw<NotSupportedException>()
                                               .WithMessage("IntBasedStructValueObject is a struct and cannot be converted from 'null'.");
      }

      [Fact]
      public void Should_return_valid_instance_if_value_type_is_reference_type_and_key_matches_the_reference_type_key()
      {
         StringBasedReferenceValueObjectTypeConverter.ConvertFrom("value").Should().BeEquivalentTo(StringBasedReferenceValueObject.Create("value"));
      }

      [Fact]
      public void Should_return_null_if_value_type_is_reference_type_and_key_matches_the_reference_type_key()
      {
         StringBasedReferenceValueObjectTypeConverter.ConvertFrom(null).Should().BeNull();
      }

      [Fact]
      public void Should_return_true_if_value_type_is_struct_and_key_type_matches_the_reference_type_key()
      {
         StringBasedStructValueObjectTypeConverter.ConvertFrom("value").Should().BeEquivalentTo(StringBasedStructValueObject.Create("value"));
      }

      [Fact]
      public void Should_throw_if_value_type_is_struct_and_string_based_key_is_null()
      {
         StringBasedStructValueObjectTypeConverter.Invoking(c => c.ConvertFrom(null))
                                                  .Should().Throw<NotSupportedException>()
                                                  .WithMessage("StringBasedStructValueObject is a struct and cannot be converted from 'null'.");
      }
   }
}

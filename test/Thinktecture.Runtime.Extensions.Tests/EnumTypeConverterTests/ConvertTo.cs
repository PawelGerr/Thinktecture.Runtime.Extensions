using System;
using FluentAssertions;
using Thinktecture.Runtime.Tests.TestEnums;
using Xunit;

namespace Thinktecture.Runtime.Tests.EnumTypeConverterTests
{
   public class ConvertTo : TypeConverterTestsBase
   {
      [Fact]
      public void Should_throw_if_null_is_provided_and_type_is_valuetype()
      {
         IntBasedConverter.Invoking(c => c.ConvertTo(null, null, null, typeof(int)))
                          .Should().Throw<NotSupportedException>()
                          .WithMessage("Int32 is a struct and cannot be converted to 'null'.");
      }

      [Fact]
      public void Should_return_default_of_provided_destinationtype_if_null_is_provided_and_type_is_referencetype()
      {
         IntBasedConverter.ConvertTo(null, null, null, typeof(string)).Should().BeNull();
      }

      [Fact]
      public void Should_return_default_of_the_key_if_null_is_provided()
      {
         IntBasedConverter.ConvertTo(null, null, null, typeof(TestEnum)).Should().BeNull();
      }

      [Fact]
      public void Should_return_key_if_type_matches_the_key()
      {
         StringBasedConverter.ConvertTo(null, null, TestEnum.Item1, typeof(string)).Should().Be("item1");
      }

      [Fact]
      public void Should_return_item_if_type_matches_the_enum()
      {
         StringBasedConverter.ConvertTo(null, null, TestEnum.Item1, typeof(TestEnum)).Should().Be(TestEnum.Item1);
      }

      [Fact]
      public void Should_throw_if_parameter_type_doesnt_match_the_enum_and_key()
      {
         StringBasedConverter.Invoking(c => c.ConvertTo(null, null, TestEnum.Item1, typeof(Guid)))
                             .Should().Throw<NotSupportedException>().WithMessage("'StringConverter' is unable to convert 'System.String' to 'System.Guid'.");
      }

      [Fact]
      public void Should_return_true_if_type_doesnt_matches_the_key_but_there_is_default_conversion_of_key_to_string()
      {
         IntBasedConverter.ConvertTo(null, null, IntegerEnum.Item1, typeof(string)).Should().Be("1");
      }

      [Fact]
      public void Should_convert_struct_enum_to_nullable_struct()
      {
         IntBasedStructEnumConverter.ConvertTo(null, null, StructIntegerEnum.Item1, typeof(StructIntegerEnum?)).Should().Be(StructIntegerEnum.Item1);
      }

      [Fact]
      public void Should_convert_null_to_nullable_struct()
      {
         IntBasedStructEnumConverter.ConvertTo(null, null, null, typeof(StructIntegerEnum?)).Should().BeNull();
      }
   }
}

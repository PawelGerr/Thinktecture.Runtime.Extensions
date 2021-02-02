using System;
using System.Collections.Generic;
using FluentAssertions;
using Thinktecture.TestEnums;
using Xunit;

namespace Thinktecture.EnumTypeConverterTests
{
   // ReSharper disable AssignNullToNotNullAttribute
   // ReSharper disable PossibleNullReferenceException
   public class ConvertFrom : TypeConverterTestsBase
   {
      [Fact]
      public void Should_return_null_if_key_is_null()
      {
         StringBasedConverter.ConvertFrom(null, null, null).Should().BeNull();
      }

      [Fact]
      public void Should_return_item_if_parameter_matches_the_key_type_and_item_exists()
      {
         StringBasedConverter.ConvertFrom(null, null, "item1").Should().Be(TestEnum.Item1);
      }

      [Fact]
      public void Should_return_invalid_item_if_parameter_matches_the_key_type_but_item_dont_exist()
      {
         var item = (TestEnum)StringBasedConverter.ConvertFrom(null, null, "item 1");
         item.Key.Should().Be("item 1");
         item.IsValid.Should().BeFalse();
      }

      [Fact]
      public void Should_return_item_if_parameter_is_enum_already()
      {
         StringBasedConverter.ConvertFrom(null, null, TestEnum.Item1).Should().Be(TestEnum.Item1);
      }

      [Fact]
      public void Should_throw_if_parameter_type_doesnt_match_the_enum_and_key()
      {
         Action action = () => StringBasedConverter.ConvertFrom(Guid.Empty);
         action.Should().Throw<NotSupportedException>();
      }

      [Fact]
      public void Should_return_true_if_type_doesnt_matches_the_key_but_there_is_default_conversion_of_key_from_string()
      {
         IntBasedConverter.ConvertFrom(null, null, "1").Should().Be(IntegerEnum.Item1);
      }

      [Fact]
      public void Should_convert_to_struct_enum()
      {
         IntBasedStructEnumConverter.ConvertFrom(null, null, 1).Should().Be(StructIntegerEnum.Item1);
      }

      [Fact]
      public void Should_throw_trying_to_convert_null_to_structs()
      {
         IntBasedStructEnumConverter.Invoking(c => c.ConvertFrom(null, null, null))
                                    .Should().Throw<NotSupportedException>().WithMessage("StructIntegerEnum is a struct and cannot be converted from 'null'.");
      }

      [Fact]
      public void Should_throw_trying_to_convert_unknown_key_of_non_validatable_enum()
      {
         ValidEnumConverter.Invoking(c => c.ConvertFrom(null, null, "invalid"))
                           .Should().Throw<FormatException>().WithMessage("There is no item of type 'ValidTestEnum' with the identifier 'invalid'.");
      }
   }
}

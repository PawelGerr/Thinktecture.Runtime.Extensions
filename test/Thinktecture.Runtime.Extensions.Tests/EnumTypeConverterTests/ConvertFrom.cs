using System;
using System.Collections.Generic;
using FluentAssertions;
using Thinktecture.Runtime.Tests.TestEnums;
using Xunit;

namespace Thinktecture.Runtime.Tests.EnumTypeConverterTests
{
   // ReSharper disable AssignNullToNotNullAttribute
   // ReSharper disable PossibleNullReferenceException
   public class ConvertFrom : TypeConverterTestsBase
   {
      [Fact]
      public void Should_return_null_if_key_is_null()
      {
         StringBasedTypeConverter.ConvertFrom(null, null, null).Should().BeNull();

         ExtensibleTestEnumTypeConverter.ConvertFrom(null, null, null).Should().BeNull();
         ExtendedTestEnumTypeConverter.ConvertFrom(null, null, null).Should().BeNull();
         DifferentAssemblyExtendedTestEnumTypeConverter.ConvertFrom(null, null, null).Should().BeNull();
      }

      [Fact]
      public void Should_return_item_if_parameter_matches_the_key_type_and_item_exists()
      {
         StringBasedTypeConverter.ConvertFrom(null, null, "item1").Should().Be(TestEnum.Item1);

         ExtensibleTestEnumTypeConverter.ConvertFrom(null, null, "item1").Should().Be(ExtensibleTestEnum.Item1);
         ExtendedTestEnumTypeConverter.ConvertFrom(null, null, "item1").Should().Be(ExtendedTestEnum.Item1);
         DifferentAssemblyExtendedTestEnumTypeConverter.ConvertFrom(null, null, "item1").Should().Be(DifferentAssemblyExtendedTestEnum.Item1);
      }

      [Fact]
      public void Should_return_invalid_item_if_parameter_matches_the_key_type_but_item_dont_exist()
      {
         var item = (TestEnum)StringBasedTypeConverter.ConvertFrom(null, null, "item 1");
         item.Key.Should().Be("item 1");
         item.IsValid.Should().BeFalse();

         var extensibleItem = (ExtensibleTestValidatableEnum)ExtensibleTestValidatableEnumTypeConverter.ConvertFrom(null, null, "item 1");
         extensibleItem.Key.Should().Be("item 1");
         extensibleItem.IsValid.Should().BeFalse();

         var extendedItem = (ExtendedTestValidatableEnum)ExtendedTestValidatableEnumTypeConverter.ConvertFrom(null, null, "item 1");
         extendedItem.Key.Should().Be("item 1");
         extendedItem.IsValid.Should().BeFalse();
      }

      [Fact]
      public void Should_return_item_if_parameter_is_enum_already()
      {
         StringBasedTypeConverter.ConvertFrom(null, null, TestEnum.Item1).Should().Be(TestEnum.Item1);

         ExtensibleTestEnumTypeConverter.ConvertFrom(null, null, ExtensibleTestEnum.Item1).Should().Be(ExtensibleTestEnum.Item1);
         ExtendedTestEnumTypeConverter.ConvertFrom(null, null, ExtendedTestEnum.Item1).Should().Be(ExtendedTestEnum.Item1);
         DifferentAssemblyExtendedTestEnumTypeConverter.ConvertFrom(null, null, DifferentAssemblyExtendedTestEnum.Item1).Should().Be(DifferentAssemblyExtendedTestEnum.Item1);
      }

      [Fact]
      public void Should_throw_if_parameter_type_doesnt_match_the_enum_and_key()
      {
         Action action = () => StringBasedTypeConverter.ConvertFrom(Guid.Empty);
         action.Should().Throw<NotSupportedException>();

         action = () => ExtensibleTestEnumTypeConverter.ConvertFrom(Guid.Empty);
         action.Should().Throw<NotSupportedException>();

         action = () => ExtendedTestEnumTypeConverter.ConvertFrom(Guid.Empty);
         action.Should().Throw<NotSupportedException>();

         action = () => DifferentAssemblyExtendedTestEnumTypeConverter.ConvertFrom(Guid.Empty);
         action.Should().Throw<NotSupportedException>();
      }

      [Fact]
      public void Should_return_true_if_type_doesnt_matches_the_key_but_there_is_default_conversion_of_key_from_string()
      {
         IntBasedTypeConverter.ConvertFrom(null, null, "1").Should().Be(IntegerEnum.Item1);
      }

      [Fact]
      public void Should_convert_to_struct_enum()
      {
         IntBasedStructEnumTypeConverter.ConvertFrom(null, null, 1).Should().Be(StructIntegerEnum.Item1);
      }

      [Fact]
      public void Should_throw_trying_to_convert_null_to_structs()
      {
         IntBasedStructEnumTypeConverter.Invoking(c => c.ConvertFrom(null, null, null))
                                        .Should().Throw<NotSupportedException>().WithMessage("StructIntegerEnum is a struct and cannot be converted from 'null'.");
      }

      [Fact]
      public void Should_throw_trying_to_convert_unknown_key_of_non_validatable_enum()
      {
         ValidEnumTypeConverter.Invoking(c => c.ConvertFrom(null, null, "invalid"))
                               .Should().Throw<FormatException>().WithMessage("There is no item of type 'ValidTestEnum' with the identifier 'invalid'.");
      }
   }
}

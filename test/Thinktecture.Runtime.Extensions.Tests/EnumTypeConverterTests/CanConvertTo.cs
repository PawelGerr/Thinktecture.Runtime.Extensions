using System;
using FluentAssertions;
using Thinktecture.TestEnums;
using Xunit;

namespace Thinktecture.EnumTypeConverterTests
{
   public class CanConvertTo : TypeConverterTestsBase
   {
      [Fact]
      public void Should_return_true_if_type_matches_the_key()
      {
         StringBasedConverter.CanConvertTo(typeof(string)).Should().BeTrue();
      }

      [Fact]
      public void Should_return_true_if_type_matches_the_enum()
      {
         StringBasedConverter.CanConvertTo(typeof(TestEnum)).Should().BeTrue();
      }

      [Fact]
      public void Should_return_false_if_type_doesnt_match_the_enum_and_key()
      {
         StringBasedConverter.CanConvertTo(typeof(Guid)).Should().BeFalse();
      }

      [Fact]
      public void Should_return_true_if_type_doesnt_matches_the_key_but_there_is_default_conversion_of_key_to_string()
      {
         IntBasedConverter.CanConvertTo(typeof(string)).Should().BeTrue();
      }

      [Fact]
      public void Should_return_true_if_type_is_nullable_struct()
      {
         IntBasedConverter.CanConvertTo(typeof(int?)).Should().BeTrue();
      }

      [Fact]
      public void Should_return_true_for_structs()
      {
         IntBasedStructEnumConverter.CanConvertTo(typeof(StructIntegerEnum)).Should().BeTrue();
      }

      [Fact]
      public void Should_return_true_for_nullable_structs()
      {
         IntBasedStructEnumConverter.CanConvertTo(typeof(StructIntegerEnum?)).Should().BeTrue();
      }
   }
}

using System;
using FluentAssertions;
using Thinktecture.Runtime.Tests.TestEnums;
using Xunit;

namespace Thinktecture.Runtime.Tests.EnumTypeConverterTests
{
   public class CanConvertFrom : TypeConverterTestsBase
   {
      [Fact]
      public void Should_return_true_if_type_matches_the_key()
      {
         StringBasedConverter.CanConvertFrom(typeof(string)).Should().BeTrue();

         ExtensibleTestEnumConverter.CanConvertFrom(typeof(string)).Should().BeTrue();
         ExtendedTestEnumConverter.CanConvertFrom(typeof(string)).Should().BeTrue();
         DifferentAssemblyExtendedTestEnumConverter.CanConvertFrom(typeof(string)).Should().BeTrue();
      }

      [Fact]
      public void Should_return_true_if_type_matches_the_enum()
      {
         StringBasedConverter.CanConvertFrom(typeof(TestEnum)).Should().BeTrue();

         ExtensibleTestEnumConverter.CanConvertFrom(typeof(ExtensibleTestEnum)).Should().BeTrue();
         ExtendedTestEnumConverter.CanConvertFrom(typeof(ExtendedTestEnum)).Should().BeTrue();
         DifferentAssemblyExtendedTestEnumConverter.CanConvertFrom(typeof(DifferentAssemblyExtendedTestEnum)).Should().BeTrue();
      }

      [Fact]
      public void Should_return_false_if_type_doesnt_match_the_enum_and_key()
      {
         StringBasedConverter.CanConvertFrom(typeof(Guid)).Should().BeFalse();

         ExtensibleTestEnumConverter.CanConvertFrom(typeof(Guid)).Should().BeFalse();
         ExtendedTestEnumConverter.CanConvertFrom(typeof(Guid)).Should().BeFalse();
         DifferentAssemblyExtendedTestEnumConverter.CanConvertFrom(typeof(Guid)).Should().BeFalse();
      }

      [Fact]
      public void Should_return_true_if_type_doesnt_matches_the_key_but_there_is_default_conversion_of_key_from_string()
      {
         IntBasedConverter.CanConvertFrom(typeof(string)).Should().BeTrue();
      }

      [Fact]
      public void Should_return_true_for_structs()
      {
         IntBasedStructEnumConverter.CanConvertFrom(typeof(StructIntegerEnum)).Should().BeTrue();
      }

      [Fact]
      public void Should_return_true_for_nullable_structs()
      {
         IntBasedStructEnumConverter.CanConvertFrom(typeof(StructIntegerEnum?)).Should().BeTrue();
      }
   }
}

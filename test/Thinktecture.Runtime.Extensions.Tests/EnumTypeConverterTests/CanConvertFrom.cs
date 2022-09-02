using System;
using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTypeConverterTests;

public class CanConvertFrom : TypeConverterTestsBase
{
   [Fact]
   public void Should_return_true_if_type_matches_the_key()
   {
      StringBasedTypeConverter.CanConvertFrom(typeof(string)).Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_if_type_matches_the_enum()
   {
      StringBasedTypeConverter.CanConvertFrom(typeof(TestEnum)).Should().BeTrue();
   }

   [Fact]
   public void Should_return_false_if_type_doesnt_match_the_enum_and_key()
   {
      StringBasedTypeConverter.CanConvertFrom(typeof(Guid)).Should().BeFalse();
   }

   [Fact]
   public void Should_return_true_if_type_doesnt_matches_the_key_but_there_is_default_conversion_of_key_from_string()
   {
      IntBasedTypeConverter.CanConvertFrom(typeof(string)).Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_for_structs()
   {
      IntBasedStructEnumTypeConverter.CanConvertFrom(typeof(StructIntegerEnum)).Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_for_nullable_structs()
   {
      IntBasedStructEnumTypeConverter.CanConvertFrom(typeof(StructIntegerEnum?)).Should().BeTrue();
   }
}

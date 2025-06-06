using System;
using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTypeConverterTests;

public class CanConvertFrom : TypeConverterTestsBase
{
   [Fact]
   public void Should_return_true_if_type_matches_the_key()
   {
      SmartEnum_StringBased_TypeConverter.CanConvertFrom(typeof(string)).Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_if_type_matches_the_enum()
   {
      SmartEnum_StringBased_TypeConverter.CanConvertFrom(typeof(SmartEnum_StringBased)).Should().BeTrue();
   }

   [Fact]
   public void Should_return_false_if_type_doesnt_match_the_enum_and_key()
   {
      SmartEnum_StringBased_TypeConverter.CanConvertFrom(typeof(Guid)).Should().BeFalse();
   }

   [Fact]
   public void Should_return_true_if_type_doesnt_matches_the_key_but_there_is_default_conversion_of_key_from_string()
   {
      SmartEnum_IntBased_TypeConverter.CanConvertFrom(typeof(string)).Should().BeTrue();
   }
}

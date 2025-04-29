using System;
using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTypeConverterTests;

public class ConvertTo : TypeConverterTestsBase
{
   [Fact]
   public void Should_throw_if_null_is_provided_and_type_is_valueobject()
   {
      SmartEnum_IntBased_TypeConverter.Invoking(c => c.ConvertTo(null, null, null, typeof(int)))
                           .Should().Throw<NotSupportedException>()
                           .WithMessage("Int32 is a struct and cannot be converted to 'null'.");
   }

   [Fact]
   public void Should_return_default_of_provided_destinationtype_if_null_is_provided_and_type_is_referencetype()
   {
      SmartEnum_IntBased_TypeConverter.ConvertTo(null, null, null, typeof(string)).Should().BeNull();
   }

   [Fact]
   public void Should_return_default_of_the_key_if_null_is_provided()
   {
      SmartEnum_IntBased_TypeConverter.ConvertTo(null, null, null, typeof(SmartEnum_StringBased)).Should().BeNull();
   }

   [Fact]
   public void Should_return_key_if_type_matches_the_key()
   {
      SmartEnum_StringBased_TypeConverter.ConvertTo(null, null, SmartEnum_StringBased.Item1, typeof(string)).Should().Be("Item1");
   }

   [Fact]
   public void Should_return_item_if_type_matches_the_enum()
   {
      SmartEnum_StringBased_TypeConverter.ConvertTo(null, null, SmartEnum_StringBased.Item1, typeof(SmartEnum_StringBased)).Should().Be(SmartEnum_StringBased.Item1);
   }

   [Fact]
   public void Should_throw_if_parameter_type_doesnt_match_the_enum_and_key()
   {
      SmartEnum_StringBased_TypeConverter.Invoking(c => c.ConvertTo(null, null, SmartEnum_StringBased.Item1, typeof(Guid)))
                              .Should().Throw<NotSupportedException>().WithMessage("'StringConverter' is unable to convert 'System.String' to 'System.Guid'.");
   }

   [Fact]
   public void Should_return_true_if_type_doesnt_matches_the_key_but_there_is_default_conversion_of_key_to_string()
   {
      SmartEnum_IntBased_TypeConverter.ConvertTo(null, null, SmartEnum_IntBased.Item1, typeof(string)).Should().Be("1");
   }
}

using System;
using System.Globalization;
using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTests;

public class FormattableTests
{
   [Fact]
   public void Should_use_format_string()
   {
      CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

      SmartEnum_DecimalBased.Value1.ToString("P").Should().Be("100.00 %");
   }

   [Fact]
   public void Should_use_format_provider()
   {
      SmartEnum_DecimalBased.Value1.ToString("P", CultureInfo.CreateSpecificCulture("de-DE")).Should().Be("100,000 %");
   }

   [Theory]
   [InlineData("N2", "en-US", "1.00")]
   [InlineData("N2", "de-DE", "1,00")]
   [InlineData("N2", "fr-FR", "1,00")]
   public void Should_format_decimal_with_numeric_format_across_cultures(string format, string cultureName, string expected)
   {
      var culture = CultureInfo.CreateSpecificCulture(cultureName);
      SmartEnum_DecimalBased.Value1.ToString(format, culture).Should().Be(expected);
   }

   [Theory]
   [InlineData("C", "en-US", "$1.00")]
   [InlineData("C", "de-DE", "1,00 €")]
   [InlineData("C", "fr-FR", "1,00 €")]
   public void Should_format_decimal_with_currency_format_across_cultures(string format, string cultureName, string expected)
   {
      var culture = CultureInfo.CreateSpecificCulture(cultureName);
      SmartEnum_DecimalBased.Value1.ToString(format, culture).Should().Be(expected);
   }

   [Fact]
   public void Should_use_null_format_string_as_default_toString()
   {
      SmartEnum_DecimalBased.Value1.ToString(null, CultureInfo.InvariantCulture).Should().Be("1");
   }

   [Fact]
   public void Should_pass_format_string_to_underlying_type()
   {
      var originalCulture = CultureInfo.CurrentCulture;

      try
      {
         CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
         SmartEnum_DecimalBased.Value1.ToString("F4").Should().Be("1.0000");
      }
      finally
      {
         CultureInfo.CurrentCulture = originalCulture;
      }
   }

   [Theory]
   [InlineData("D", "en-US", "Monday, January 1, 2024")]
   [InlineData("d", "en-US", "1/1/2024")]
   [InlineData("D", "de-DE", "Montag, 1. Januar 2024")]
   [InlineData("d", "de-DE", "01.01.2024")]
   public void Should_format_datetime_with_different_format_strings(string format, string cultureName, string expected)
   {
      var culture = CultureInfo.CreateSpecificCulture(cultureName);
      SmartEnum_DateTimeBased.Item1.ToString(format, culture).Should().Be(expected);
   }

   [Fact]
   public void Should_format_datetime_with_custom_format_string()
   {
      SmartEnum_DateTimeBased.Item1.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture).Should().Be("2024-01-01");
   }

   [Theory]
   [InlineData("en-US", "Monday, January 1, 2024")]
   [InlineData("de-DE", "Montag, 1. Januar 2024")]
   [InlineData("fr-FR", "lundi 1 janvier 2024")]
   public void Should_format_datetime_long_date_pattern_across_cultures(string cultureName, string expected)
   {
      var culture = CultureInfo.CreateSpecificCulture(cultureName);
      SmartEnum_DateTimeBased.Item1.ToString("D", culture).Should().Be(expected);
   }
}

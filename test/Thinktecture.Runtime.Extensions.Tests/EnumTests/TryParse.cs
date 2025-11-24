using System;
using System.Globalization;
using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTests;

public class TryParse
{
   [Fact]
   public void Should_parse_valid_value()
   {
      SmartEnum_IntBased.TryParse(SmartEnum_IntBased.Item1.Key.ToString(), null, out var intItem).Should().BeTrue();
      intItem.Should().Be(SmartEnum_IntBased.Item1);

      SmartEnum_IntBased_WithStringObjectFactory.TryParse(SmartEnum_IntBased_WithStringObjectFactory.Item1.Key.ToString(), null, out var intItemWithStringFactory).Should().BeTrue();
      intItemWithStringFactory.Should().Be(SmartEnum_IntBased_WithStringObjectFactory.Item1);

#if NET9_0_OR_GREATER
      SmartEnum_IntBased_WithReadOnlySpanOfCharObjectFactory.TryParse(SmartEnum_IntBased_WithReadOnlySpanOfCharObjectFactory.Item1.Key.ToString(), null, out var intItemWithSpanFactory).Should().BeTrue();
      intItemWithSpanFactory.Should().Be(SmartEnum_IntBased_WithReadOnlySpanOfCharObjectFactory.Item1);

      SmartEnum_IntBased_WithStringAndReadOnlySpanOfCharObjectFactory.TryParse(SmartEnum_IntBased_WithStringAndReadOnlySpanOfCharObjectFactory.Item1.Key.ToString(), null, out var intItemWithBothFactories).Should().BeTrue();
      intItemWithBothFactories.Should().Be(SmartEnum_IntBased_WithStringAndReadOnlySpanOfCharObjectFactory.Item1);
#endif

      SmartEnum_DecimalBased.TryParse("1", null, out var item).Should().BeTrue();
      item.Should().Be(SmartEnum_DecimalBased.Value1);

      SmartEnum_DecimalBased.TryParse("1.0", CultureInfo.InvariantCulture, out item).Should().BeTrue();
      item.Should().Be(SmartEnum_DecimalBased.Value1);

      SmartEnum_StringBased.TryParse(SmartEnum_StringBased.Item1.Key, null, out var stringItem).Should().BeTrue();
      stringItem.Should().Be(SmartEnum_StringBased.Item1);

      SmartEnum_StringBased_WithStringBasedObjectFactory.TryParse(SmartEnum_StringBased_WithStringBasedObjectFactory.Item1, null, out var stringItemWithFactory).Should().BeTrue();
      stringItemWithFactory.Should().Be(SmartEnum_StringBased_WithStringBasedObjectFactory.Item1);

      SmartEnum_StringBased_WithStringAndReadOnlySpanOfCharObjectFactory.TryParse(SmartEnum_StringBased_WithStringAndReadOnlySpanOfCharObjectFactory.Item1, null, out var stringItemWithBothFactories).Should().BeTrue();
      stringItemWithBothFactories.Should().Be(SmartEnum_StringBased_WithStringAndReadOnlySpanOfCharObjectFactory.Item1);

      SmartEnum_CaseSensitive.TryParse("item", null, out var caseSensitiveItem).Should().BeTrue();
      caseSensitiveItem.Should().Be(SmartEnum_CaseSensitive.LowerCased);
   }

#if NET9_0_OR_GREATER
   [Fact]
   public void Should_parse_ReadOnlySpanOfChar()
   {
      SmartEnum_IntBased.TryParse(SmartEnum_IntBased.Item1.Key.ToString().AsSpan(), null, out var intItem).Should().BeTrue();
      intItem.Should().Be(SmartEnum_IntBased.Item1);

      SmartEnum_IntBased_WithStringObjectFactory.TryParse(SmartEnum_IntBased_WithStringObjectFactory.Item1.Key.ToString().AsSpan(), null, out var intItemWithStringFactory).Should().BeTrue();
      intItemWithStringFactory.Should().Be(SmartEnum_IntBased_WithStringObjectFactory.Item1);

      SmartEnum_IntBased_WithReadOnlySpanOfCharObjectFactory.TryParse(SmartEnum_IntBased_WithReadOnlySpanOfCharObjectFactory.Item1.Key.ToString().AsSpan(), null, out var intItemWithSpanFactory).Should().BeTrue();
      intItemWithSpanFactory.Should().Be(SmartEnum_IntBased_WithReadOnlySpanOfCharObjectFactory.Item1);

      SmartEnum_IntBased_WithStringAndReadOnlySpanOfCharObjectFactory.TryParse(SmartEnum_IntBased_WithStringAndReadOnlySpanOfCharObjectFactory.Item1.Key.ToString().AsSpan(), null, out var intItemWithBothFactories).Should().BeTrue();
      intItemWithBothFactories.Should().Be(SmartEnum_IntBased_WithStringAndReadOnlySpanOfCharObjectFactory.Item1);

      SmartEnum_DecimalBased.TryParse("1".AsSpan(), null, out var item).Should().BeTrue();
      item.Should().Be(SmartEnum_DecimalBased.Value1);

      SmartEnum_DecimalBased.TryParse("1.0".AsSpan(), CultureInfo.InvariantCulture, out item).Should().BeTrue();
      item.Should().Be(SmartEnum_DecimalBased.Value1);

      SmartEnum_StringBased.TryParse(SmartEnum_StringBased.Item1.Key.AsSpan(), null, out var stringItem).Should().BeTrue();
      stringItem.Should().Be(SmartEnum_StringBased.Item1);

      SmartEnum_StringBased_WithStringBasedObjectFactory.TryParse(SmartEnum_StringBased_WithStringBasedObjectFactory.Item1.Key.AsSpan(), null, out var stringItemWithFactory).Should().BeTrue();
      stringItemWithFactory.Should().Be(SmartEnum_StringBased_WithStringBasedObjectFactory.Item1);

      SmartEnum_StringBased_WithStringAndReadOnlySpanOfCharObjectFactory.TryParse(SmartEnum_StringBased_WithStringAndReadOnlySpanOfCharObjectFactory.Item1.Key.AsSpan(), null, out var stringItemWithBothFactories).Should().BeTrue();
      stringItemWithBothFactories.Should().Be(SmartEnum_StringBased_WithStringAndReadOnlySpanOfCharObjectFactory.Item1);
   }
#endif

   [Fact]
   public void Should_use_format_provider_parse_valid_value()
   {
      var german = CultureInfo.CreateSpecificCulture("de-DE");
      SmartEnum_DecimalBased.TryParse("1", german, out var item).Should().BeTrue();
      item.Should().Be(SmartEnum_DecimalBased.Value1);

      SmartEnum_DecimalBased.TryParse("1,0", german, out item).Should().BeTrue();
      item.Should().Be(SmartEnum_DecimalBased.Value1);

      SmartEnum_CaseSensitive.TryParse("item", german, out var caseSensitiveItem).Should().BeTrue();
      caseSensitiveItem.Should().Be(SmartEnum_CaseSensitive.LowerCased);
   }

   [Fact]
   public void Should_return_false_if_string_null()
   {
      SmartEnum_DecimalBased.TryParse(null, null, out var item).Should().BeFalse();
      item.Should().BeNull();
   }

   [Fact]
   public void Should_return_false_if_string_is_not_parsable_to_key_type()
   {
      SmartEnum_DecimalBased.TryParse(String.Empty, null, out var item).Should().BeFalse();
      item.Should().BeNull();

      SmartEnum_DecimalBased.TryParse("invalid", null, out item).Should().BeFalse();
      item.Should().BeNull();

      SmartEnum_CaseSensitive.TryParse("invalid", null, out var caseSensitiveItem).Should().BeFalse();
      caseSensitiveItem.Should().BeNull();

      SmartEnum_CaseSensitive.TryParse("ITEM2", null, out caseSensitiveItem).Should().BeFalse();
      caseSensitiveItem.Should().BeNull();
   }

   [Fact]
   public void Should_support_TryParse_for_generic_string_based_enum()
   {
      SmartEnum_Generic_StringBased<int>.TryParse("item1", null, out var item).Should().BeTrue();
      item.Should().BeSameAs(SmartEnum_Generic_StringBased<int>.Item1);

      SmartEnum_Generic_StringBased<int>.TryParse("invalid", null, out item).Should().BeFalse();
      item.Should().BeNull();
   }

   [Theory]
   [InlineData("en-US", "1.0")]
   [InlineData("de-DE", "1,0")]
   [InlineData("fr-FR", "1,0")]
   public void Should_try_parse_decimal_with_culture_specific_separators(string cultureName, string input)
   {
      var culture = CultureInfo.CreateSpecificCulture(cultureName);
      var success = SmartEnum_DecimalBased.TryParse(input, culture, out var result);
      success.Should().BeTrue();
      result.Should().Be(SmartEnum_DecimalBased.Value1);
   }

   [Fact]
   public void Should_try_parse_decimal_with_invariant_culture()
   {
      SmartEnum_DecimalBased.TryParse("2.0", CultureInfo.InvariantCulture, out var result).Should().BeTrue();
      result.Should().Be(SmartEnum_DecimalBased.Value2);
   }

   [Fact]
   public void Should_try_parse_decimal_with_null_provider_uses_current_culture()
   {
      var originalCulture = CultureInfo.CurrentCulture;
      try
      {
         CultureInfo.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
         SmartEnum_DecimalBased.TryParse("1.5", null, out var result).Should().BeTrue();
         result!.Key.Should().Be(1.5m);
      }
      finally
      {
         CultureInfo.CurrentCulture = originalCulture;
      }
   }

   [Theory]
   [InlineData("en-US", "01/01/2024")]
   [InlineData("de-DE", "01.01.2024")]
   [InlineData("fr-FR", "01/01/2024")]
   public void Should_try_parse_datetime_with_culture_specific_formats(string cultureName, string input)
   {
      var culture = CultureInfo.CreateSpecificCulture(cultureName);
      var expected = new DateTime(2024, 1, 1);
      var success = SmartEnum_DateTimeBased.TryParse(input, culture, out var result);
      success.Should().BeTrue();
      result!.Key.Should().Be(expected);
   }

   [Fact]
   public void Should_try_parse_datetime_with_invariant_culture()
   {
      var expected = new DateTime(2024, 1, 1);
      SmartEnum_DateTimeBased.TryParse("01/01/2024", CultureInfo.InvariantCulture, out var result).Should().BeTrue();
      result!.Key.Should().Be(expected);
   }

#if NET9_0_OR_GREATER
   [Fact]
   public void Should_try_parse_span_with_custom_ISpanParsable_key()
   {
      var span = "2".AsSpan();
      var success = SmartEnum_CustomSpanParsableKey.TryParse(span, null, out var result);
      success.Should().BeTrue();
      result.Should().Be(SmartEnum_CustomSpanParsableKey.Item2);
   }

   [Fact]
   public void Should_try_parse_span_return_false_for_invalid_custom_ISpanParsable_key()
   {
      var span = "999".AsSpan();
      var success = SmartEnum_CustomSpanParsableKey.TryParse(span, null, out var result);
      success.Should().BeFalse();
      result.Should().BeNull();
   }

   [Fact]
   public void Should_try_parse_string_with_custom_ISpanParsable_key()
   {
      var success = SmartEnum_CustomSpanParsableKey.TryParse("2", null, out var result);
      success.Should().BeTrue();
      result.Should().Be(SmartEnum_CustomSpanParsableKey.Item2);
   }

   [Fact]
   public void Should_try_parse_string_return_false_for_invalid_custom_ISpanParsable_key()
   {
      var success = SmartEnum_CustomSpanParsableKey.TryParse("999", null, out var result);
      success.Should().BeFalse();
      result.Should().BeNull();
   }

   [Theory]
   [InlineData("en-US", "2.0")]
   [InlineData("de-DE", "2,0")]
   public void Should_try_parse_span_decimal_with_culture(string cultureName, string input)
   {
      var culture = CultureInfo.CreateSpecificCulture(cultureName);
      var success = SmartEnum_DecimalBased.TryParse(input.AsSpan(), culture, out var result);
      success.Should().BeTrue();
      result.Should().Be(SmartEnum_DecimalBased.Value2);
   }

   [Theory]
   [InlineData("en-US", "invalid")]
   [InlineData("de-DE", "invalid")]
   public void Should_try_parse_span_return_false_with_invalid_decimal_for_culture(string cultureName, string input)
   {
      var culture = CultureInfo.CreateSpecificCulture(cultureName);
      var success = SmartEnum_DecimalBased.TryParse(input.AsSpan(), culture, out var result);
      success.Should().BeFalse();
      result.Should().BeNull();
   }

   [Theory]
   [InlineData("en-US", "12/31/2024")]
   [InlineData("de-DE", "31.12.2024")]
   public void Should_try_parse_span_datetime_with_culture(string cultureName, string input)
   {
      var culture = CultureInfo.CreateSpecificCulture(cultureName);
      var expected = new DateTime(2024, 12, 31);
      var success = SmartEnum_DateTimeBased.TryParse(input.AsSpan(), culture, out var result);
      success.Should().BeTrue();
      result!.Key.Should().Be(expected);
   }

   [Fact]
   public void Should_try_parse_span_return_false_for_empty_span()
   {
      var success = SmartEnum_IntBased.TryParse(ReadOnlySpan<char>.Empty, null, out var result);

      success.Should().BeFalse();
      result.Should().BeNull();
   }

   [Fact]
   public void Should_try_parse_span_return_false_for_invalid_malformed_decimal()
   {
      var success = SmartEnum_DecimalBased.TryParse("12.34.56".AsSpan(), CultureInfo.InvariantCulture, out var result);

      success.Should().BeFalse();
      result.Should().BeNull();
   }

   [Fact]
   public void Should_try_parse_span_return_false_for_valid_key_not_in_enum()
   {
      var validKeyNotInEnum = "999".AsSpan(); // Valid int, but not in enum

      var success = SmartEnum_IntBased.TryParse(validKeyNotInEnum, null, out var result);

      success.Should().BeFalse();
      result.Should().BeNull();
   }
#endif

   [Fact]
   public void Should_try_parse_return_false_for_valid_key_not_in_enum()
   {
      var validKeyNotInEnum = "999"; // Valid int, but not in enum

      var success = SmartEnum_IntBased.TryParse(validKeyNotInEnum, null, out var result);

      success.Should().BeFalse();
      result.Should().BeNull();
   }

   [Fact]
   public void Should_try_parse_return_false_for_malformed_decimal()
   {
      var invalidDecimal = "12.34.56"; // Invalid format

      var success = SmartEnum_DecimalBased.TryParse(invalidDecimal, CultureInfo.InvariantCulture, out var result);

      success.Should().BeFalse();
      result.Should().BeNull();
   }

   [Fact]
   public void Should_try_parse_return_false_for_string_enum_with_invalid_key()
   {
      var invalidKey = "NonExistentItem";

      var success = SmartEnum_StringBased.TryParse(invalidKey, null, out var result);

      success.Should().BeFalse();
      result.Should().BeNull();
   }
}

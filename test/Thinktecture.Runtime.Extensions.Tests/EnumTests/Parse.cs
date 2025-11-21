using System;
using System.Globalization;
using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTests;

public class Parse
{
   [Fact]
   public void Should_parse_valid_value()
   {
      SmartEnum_IntBased.Parse(SmartEnum_IntBased.Item1.Key.ToString(), null).Should().Be(SmartEnum_IntBased.Item1);

      SmartEnum_IntBased_WithStringObjectFactory.Parse(SmartEnum_IntBased_WithStringObjectFactory.Item1.Key.ToString(), null).Should().Be(SmartEnum_IntBased_WithStringObjectFactory.Item1);

#if NET9_0_OR_GREATER
      SmartEnum_IntBased_WithReadOnlySpanOfCharObjectFactory.Parse(SmartEnum_IntBased_WithReadOnlySpanOfCharObjectFactory.Item1.Key.ToString(), null).Should().Be(SmartEnum_IntBased_WithReadOnlySpanOfCharObjectFactory.Item1);

      SmartEnum_IntBased_WithStringAndReadOnlySpanOfCharObjectFactory.Parse(SmartEnum_IntBased_WithStringAndReadOnlySpanOfCharObjectFactory.Item1.Key.ToString(), null).Should().Be(SmartEnum_IntBased_WithStringAndReadOnlySpanOfCharObjectFactory.Item1);
#endif

      SmartEnum_DecimalBased.Parse("1", null).Should().Be(SmartEnum_DecimalBased.Value1);
      SmartEnum_DecimalBased.Parse("1.0", CultureInfo.InvariantCulture).Should().Be(SmartEnum_DecimalBased.Value1);

      SmartEnum_StringBased.Parse(SmartEnum_StringBased.Item1.Key, null)
                           .Should().Be(SmartEnum_StringBased.Item1);

      SmartEnum_StringBased_WithStringBasedObjectFactory.Parse(SmartEnum_StringBased_WithStringBasedObjectFactory.Item1, null)
                                                        .Should().Be(SmartEnum_StringBased_WithStringBasedObjectFactory.Item1);

      SmartEnum_StringBased_WithStringAndReadOnlySpanOfCharObjectFactory.Parse(SmartEnum_StringBased_WithStringAndReadOnlySpanOfCharObjectFactory.Item1, null)
                                                                        .Should().Be(SmartEnum_StringBased_WithStringAndReadOnlySpanOfCharObjectFactory.Item1);
   }

#if NET9_0_OR_GREATER
   [Fact]
   public void Should_parse_ReadOnlySpanOfChar()
   {
      SmartEnum_IntBased.Parse(SmartEnum_IntBased.Item1.Key.ToString().AsSpan(), null).Should().Be(SmartEnum_IntBased.Item1);

      SmartEnum_IntBased_WithStringObjectFactory.Parse(SmartEnum_IntBased_WithStringObjectFactory.Item1.Key.ToString().AsSpan(), null).Should().Be(SmartEnum_IntBased_WithStringObjectFactory.Item1);

      SmartEnum_IntBased_WithReadOnlySpanOfCharObjectFactory.Parse(SmartEnum_IntBased_WithReadOnlySpanOfCharObjectFactory.Item1.Key.ToString().AsSpan(), null).Should().Be(SmartEnum_IntBased_WithReadOnlySpanOfCharObjectFactory.Item1);

      SmartEnum_IntBased_WithStringAndReadOnlySpanOfCharObjectFactory.Parse(SmartEnum_IntBased_WithStringAndReadOnlySpanOfCharObjectFactory.Item1.Key.ToString().AsSpan(), null).Should().Be(SmartEnum_IntBased_WithStringAndReadOnlySpanOfCharObjectFactory.Item1);

      SmartEnum_DecimalBased.Parse("1".AsSpan(), null).Should().Be(SmartEnum_DecimalBased.Value1);
      SmartEnum_DecimalBased.Parse("1.0".AsSpan(), CultureInfo.InvariantCulture).Should().Be(SmartEnum_DecimalBased.Value1);

      SmartEnum_StringBased.Parse(SmartEnum_StringBased.Item1.Key.AsSpan(), null)
                           .Should().Be(SmartEnum_StringBased.Item1);

      SmartEnum_StringBased_WithStringBasedObjectFactory.Parse(SmartEnum_StringBased_WithStringBasedObjectFactory.Item1.Key.AsSpan(), null)
                                                        .Should().Be(SmartEnum_StringBased_WithStringBasedObjectFactory.Item1);

      SmartEnum_StringBased_WithStringAndReadOnlySpanOfCharObjectFactory.Parse(SmartEnum_StringBased_WithStringAndReadOnlySpanOfCharObjectFactory.Item1.Key.AsSpan(), null)
                                                                        .Should().Be(SmartEnum_StringBased_WithStringAndReadOnlySpanOfCharObjectFactory.Item1);
   }
#endif

   [Fact]
   public void Should_use_format_provider_parse_valid_value()
   {
      var german = CultureInfo.CreateSpecificCulture("de-DE");

      SmartEnum_DecimalBased.Parse("1", german).Should().Be(SmartEnum_DecimalBased.Value1);

      SmartEnum_DecimalBased.Parse("1,0", german).Should().Be(SmartEnum_DecimalBased.Value1);
   }

   [Theory]
   [InlineData("en-US", "1.0")]
   [InlineData("de-DE", "1,0")]
   [InlineData("fr-FR", "1,0")]
   public void Should_parse_decimal_with_culture_specific_separators(string cultureName, string input)
   {
      var culture = CultureInfo.CreateSpecificCulture(cultureName);
      var result = SmartEnum_DecimalBased.Parse(input, culture);
      result.Should().Be(SmartEnum_DecimalBased.Value1);
   }

   [Fact]
   public void Should_parse_decimal_with_invariant_culture()
   {
      SmartEnum_DecimalBased.Parse("2.0", CultureInfo.InvariantCulture).Should().Be(SmartEnum_DecimalBased.Value2);
   }

   [Fact]
   public void Should_parse_decimal_with_null_provider_uses_current_culture()
   {
      var originalCulture = CultureInfo.CurrentCulture;
      try
      {
         CultureInfo.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
         SmartEnum_DecimalBased.Parse("1.5", null).Key.Should().Be(1.5m);
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
   public void Should_parse_datetime_with_culture_specific_formats(string cultureName, string input)
   {
      var culture = CultureInfo.CreateSpecificCulture(cultureName);
      var expected = new DateTime(2024, 1, 1);
      var result = SmartEnum_DateTimeBased.Parse(input, culture);
      result.Key.Should().Be(expected);
   }

   [Fact]
   public void Should_parse_datetime_with_invariant_culture()
   {
      var expected = new DateTime(2024, 1, 1);
      SmartEnum_DateTimeBased.Parse("01/01/2024", CultureInfo.InvariantCulture).Key.Should().Be(expected);
   }

   [Fact]
   public void Should_return_false_if_string_null()
   {
      FluentActions.Invoking(() => SmartEnum_DecimalBased.Parse(null!, null))
                   .Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 's')");
   }

   [Fact]
   public void Should_return_false_if_string_is_not_parsable_to_key_type()
   {
      FluentActions.Invoking(() => SmartEnum_DecimalBased.Parse(String.Empty, null))
                   .Should().Throw<FormatException>().WithMessage("The input string '' was not in a correct format.");

      FluentActions.Invoking(() => SmartEnum_DecimalBased.Parse("invalid", null))
                   .Should().Throw<FormatException>().WithMessage("The input string 'invalid' was not in a correct format.");
   }

   [Fact]
   public void Should_support_Parse_for_generic_string_based_enum()
   {
      var item = SmartEnum_Generic_StringBased<int>.Parse("item1", null);
      item.Should().BeSameAs(SmartEnum_Generic_StringBased<int>.Item1);

      item = SmartEnum_Generic_StringBased<int>.Parse("item2", null);
      item.Should().BeSameAs(SmartEnum_Generic_StringBased<int>.Item2);
   }

   [Fact]
   public void Should_throw_on_Parse_with_invalid_key_for_generic_string_based_enum()
   {
      var action = () => SmartEnum_Generic_StringBased<int>.Parse("invalid", null);
      action.Should().Throw<FormatException>();
   }

#if NET9_0_OR_GREATER
   [Fact]
   public void Should_parse_span_with_custom_ISpanParsable_key()
   {
      var span = "1".AsSpan();
      var result = SmartEnum_CustomSpanParsableKey.Parse(span, null);
      result.Should().Be(SmartEnum_CustomSpanParsableKey.Item1);
   }

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
   public void Should_parse_string_with_custom_ISpanParsable_key()
   {
      var result = SmartEnum_CustomSpanParsableKey.Parse("1", null);
      result.Should().Be(SmartEnum_CustomSpanParsableKey.Item1);
   }

   [Fact]
   public void Should_try_parse_string_with_custom_ISpanParsable_key()
   {
      var success = SmartEnum_CustomSpanParsableKey.TryParse("2", null, out var result);
      success.Should().BeTrue();
      result.Should().Be(SmartEnum_CustomSpanParsableKey.Item2);
   }

   [Theory]
   [InlineData("en-US", "1.0")]
   [InlineData("de-DE", "1,0")]
   [InlineData("fr-FR", "1,0")]
   public void Should_parse_span_decimal_with_culture_specific_separators(string cultureName, string input)
   {
      var culture = CultureInfo.CreateSpecificCulture(cultureName);
      var result = SmartEnum_DecimalBased.Parse(input.AsSpan(), culture);
      result.Should().Be(SmartEnum_DecimalBased.Value1);
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
   [InlineData("en-US", "01/01/2024")]
   [InlineData("de-DE", "01.01.2024")]
   public void Should_parse_span_datetime_with_culture_specific_formats(string cultureName, string input)
   {
      var culture = CultureInfo.CreateSpecificCulture(cultureName);
      var expected = new DateTime(2024, 1, 1);
      var result = SmartEnum_DateTimeBased.Parse(input.AsSpan(), culture);
      result.Key.Should().Be(expected);
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
   public void Should_parse_span_throw_FormatException_for_empty_span()
   {
      FluentActions.Invoking(() => SmartEnum_IntBased.Parse(ReadOnlySpan<char>.Empty, null))
                   .Should().Throw<FormatException>();
   }

   [Fact]
   public void Should_try_parse_span_return_false_for_empty_span()
   {
      var success = SmartEnum_IntBased.TryParse(ReadOnlySpan<char>.Empty, null, out var result);

      success.Should().BeFalse();
      result.Should().BeNull();
   }

   [Fact]
   public void Should_parse_span_throw_FormatException_for_invalid_malformed_decimal()
   {
      FluentActions.Invoking(() => SmartEnum_DecimalBased.Parse("12.34.56".AsSpan(), CultureInfo.InvariantCulture))
                   .Should().Throw<FormatException>();
   }

   [Fact]
   public void Should_try_parse_span_return_false_for_invalid_malformed_decimal()
   {
      var success = SmartEnum_DecimalBased.TryParse("12.34.56".AsSpan(), CultureInfo.InvariantCulture, out var result);

      success.Should().BeFalse();
      result.Should().BeNull();
   }

   [Fact]
   public void Should_parse_span_throw_FormatException_for_valid_key_not_in_enum()
   {
      FluentActions.Invoking(() => SmartEnum_IntBased.Parse("999".AsSpan(), null))
                   .Should().Throw<FormatException>().WithMessage("There is no item of type 'SmartEnum_IntBased' with the identifier '999'.");
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
   public void Should_parse_throw_FormatException_for_valid_key_not_in_enum()
   {
      var validKeyNotInEnum = "999"; // Valid int, but not in enum

      FluentActions.Invoking(() => SmartEnum_IntBased.Parse(validKeyNotInEnum, null))
                   .Should().Throw<FormatException>().WithMessage("There is no item of type 'SmartEnum_IntBased' with the identifier '999'.");
   }

   [Fact]
   public void Should_parse_throw_FormatException_for_malformed_decimal_multiple_separators()
   {
      var invalidDecimal = "12.34.56"; // Invalid format

      FluentActions.Invoking(() => SmartEnum_DecimalBased.Parse(invalidDecimal, CultureInfo.InvariantCulture))
                   .Should().Throw<FormatException>();
   }

   [Fact]
   public void Should_parse_throw_FormatException_with_type_name_for_string_enum()
   {
      var invalidKey = "NonExistentItem";

      FluentActions.Invoking(() => SmartEnum_StringBased.Parse(invalidKey, null))
                   .Should().Throw<FormatException>().WithMessage("There is no item of type 'SmartEnum_StringBased' with the identifier 'NonExistentItem'.");
   }
}

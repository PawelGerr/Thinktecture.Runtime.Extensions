using System;
using System.Globalization;
using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.ValueObjectTests;

public class Parse
{
   [Fact]
   public void Should_parse_valid_value()
   {
      IntBasedReferenceValueObject.Parse("1", null).Should().Be(IntBasedReferenceValueObject.Create(1));
      IntBasedReferenceValueObject_With_StringBasedObjectFactory.Parse("1", null).Should().Be(IntBasedReferenceValueObject_With_StringBasedObjectFactory.Create(1));

      StringBasedReferenceValueObject.Parse("test", null).Should().Be(StringBasedReferenceValueObject.Create("test"));
      StringBasedReferenceValueObject_With_StringBasedObjectFactory.Parse("test", null).Should().Be(StringBasedReferenceValueObject_With_StringBasedObjectFactory.Create("test"));

#if NET9_0_OR_GREATER
      IntBasedReferenceValueObject_With_ReadOnlySpanOfCharBasedObjectFactory.Parse("1", null).Should().Be(IntBasedReferenceValueObject_With_ReadOnlySpanOfCharBasedObjectFactory.Create(1));
      IntBasedReferenceValueObject_With_StringAndReadOnlySpanOfCharBasedObjectFactory.Parse("1", null).Should().Be(IntBasedReferenceValueObject_With_StringAndReadOnlySpanOfCharBasedObjectFactory.Create(1));

      StringBasedReferenceValueObject_With_ReadOnlyBasedObjectFactory.Parse("test", null).Should().Be(StringBasedReferenceValueObject_With_ReadOnlyBasedObjectFactory.Create("test"));
      StringBasedReferenceValueObject_With_StringAndReadOnlyBasedObjectFactory.Parse("test", null).Should().Be(StringBasedReferenceValueObject_With_StringAndReadOnlyBasedObjectFactory.Create("test"));
#endif
      DecimalBasedClassValueObject.Parse("1", null).Should().Be(DecimalBasedClassValueObject.Create(1));

      DecimalBasedClassValueObject.Parse("1.0", CultureInfo.InvariantCulture).Should().Be(DecimalBasedClassValueObject.Create(1));
   }

#if NET9_0_OR_GREATER
   [Fact]
   public void Should_parse_ReadOnlySpanOfChar()
   {
      IntBasedReferenceValueObject.Parse(1.ToString().AsSpan(), null).Should().Be(IntBasedReferenceValueObject.Create(1));
      IntBasedReferenceValueObject_With_StringBasedObjectFactory.Parse("1".AsSpan(), null).Should().Be(IntBasedReferenceValueObject_With_StringBasedObjectFactory.Create(1));
      IntBasedReferenceValueObject_With_ReadOnlySpanOfCharBasedObjectFactory.Parse("1".AsSpan(), null).Should().Be(IntBasedReferenceValueObject_With_ReadOnlySpanOfCharBasedObjectFactory.Create(1));
      IntBasedReferenceValueObject_With_StringAndReadOnlySpanOfCharBasedObjectFactory.Parse("1".AsSpan(), null).Should().Be(IntBasedReferenceValueObject_With_StringAndReadOnlySpanOfCharBasedObjectFactory.Create(1));

      StringBasedReferenceValueObject.Parse("test".AsSpan(), null).Should().Be(StringBasedReferenceValueObject.Create("test"));
      StringBasedReferenceValueObject_With_StringBasedObjectFactory.Parse("test".AsSpan(), null).Should().Be(StringBasedReferenceValueObject_With_StringBasedObjectFactory.Create("test"));
      StringBasedReferenceValueObject_With_ReadOnlyBasedObjectFactory.Parse("test".AsSpan(), null).Should().Be(StringBasedReferenceValueObject_With_ReadOnlyBasedObjectFactory.Create("test"));
      StringBasedReferenceValueObject_With_StringAndReadOnlyBasedObjectFactory.Parse("test".AsSpan(), null).Should().Be(StringBasedReferenceValueObject_With_StringAndReadOnlyBasedObjectFactory.Create("test"));

      DecimalBasedClassValueObject.Parse("1".AsSpan(), null).Should().Be(DecimalBasedClassValueObject.Create(1));

      DecimalBasedClassValueObject.Parse("1.0".AsSpan(), CultureInfo.InvariantCulture).Should().Be(DecimalBasedClassValueObject.Create(1));
   }
#endif

   [Fact]
   public void Should_parse_valid_value_having_custom_factory_method()
   {
      IntBasedReferenceValueObjectWithCustomFactoryNames.Parse("1", null).Should().Be(IntBasedReferenceValueObjectWithCustomFactoryNames.Get(1));
   }

   [Fact]
   public void Should_use_format_provider_parse_valid_value()
   {
      var german = CultureInfo.CreateSpecificCulture("de-DE");

      DecimalBasedClassValueObject.Parse("1", german).Should().Be(DecimalBasedClassValueObject.Create(1));

      DecimalBasedClassValueObject.Parse("1,0", german).Should().Be(DecimalBasedClassValueObject.Create(1));
   }

   [Theory]
   [InlineData("en-US", "1234.56")]
   [InlineData("de-DE", "1234,56")]
   [InlineData("fr-FR", "1234,56")]
   public void Should_parse_decimal_with_culture_specific_separators(string cultureName, string input)
   {
      var culture = CultureInfo.CreateSpecificCulture(cultureName);
      var result = DecimalBasedClassValueObject.Parse(input, culture);
      result.Property.Should().Be(1234.56m);
   }

   [Fact]
   public void Should_parse_decimal_with_invariant_culture()
   {
      DecimalBasedClassValueObject.Parse("2.5", CultureInfo.InvariantCulture).Property.Should().Be(2.5m);
   }

   [Fact]
   public void Should_parse_decimal_with_null_provider_uses_current_culture()
   {
      var originalCulture = CultureInfo.CurrentCulture;
      try
      {
         CultureInfo.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
         DecimalBasedClassValueObject.Parse("1.5", null).Property.Should().Be(1.5m);
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
      var expected = TestValueObjectDateTime.Create(new DateTime(2024, 1, 1));
      var result = TestValueObjectDateTime.Parse(input, culture);
      result.Should().Be(expected);
   }

   [Fact]
   public void Should_parse_datetime_with_invariant_culture()
   {
      var expected = TestValueObjectDateTime.Create(new DateTime(2024, 1, 1));
      TestValueObjectDateTime.Parse("01/01/2024", CultureInfo.InvariantCulture).Should().Be(expected);
   }

   [Fact]
   public void Should_return_false_if_string_null()
   {
      FluentActions.Invoking(() => DecimalBasedClassValueObject.Parse(null!, null))
                   .Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 's')");
   }

   [Fact]
   public void Should_return_false_if_string_is_not_parsable_to_key_type()
   {
      FluentActions.Invoking(() => DecimalBasedClassValueObject.Parse(String.Empty, null))
                   .Should().Throw<FormatException>().WithMessage("The input string '' was not in a correct format.");

      FluentActions.Invoking(() => DecimalBasedClassValueObject.Parse("invalid", null))
                   .Should().Throw<FormatException>().WithMessage("The input string 'invalid' was not in a correct format.");
   }

   [Fact]
   public void Should_support_Parse_for_generic_string_based_value_objects()
   {
      var obj = ValueObject_Generic_StringBased<object>.Parse("test", null);

      obj.Value.Should().Be("test");
   }

#if NET9_0_OR_GREATER
   [Fact]
   public void Should_parse_span_with_custom_ISpanParsable_key()
   {
      var span = "1".AsSpan();
      var result = ValueObject_CustomSpanParsableKey.Parse(span, null);
      result.Should().NotBeNull();
      result!.Value.Value.Should().Be(1);
   }

   [Fact]
   public void Should_try_parse_span_with_custom_ISpanParsable_key()
   {
      var span = "2".AsSpan();
      var success = ValueObject_CustomSpanParsableKey.TryParse(span, null, out var result);
      success.Should().BeTrue();
      result.Should().NotBeNull();
      result!.Value.Value.Should().Be(2);
   }

   [Fact]
   public void Should_try_parse_span_return_false_for_invalid_custom_ISpanParsable_key()
   {
      var span = "invalid".AsSpan();
      var success = ValueObject_CustomSpanParsableKey.TryParse(span, null, out var result);
      success.Should().BeFalse();
      result.Should().BeNull();
   }

   [Fact]
   public void Should_parse_string_with_custom_ISpanParsable_key()
   {
      var result = ValueObject_CustomSpanParsableKey.Parse("1", null);
      result.Should().NotBeNull();
      result!.Value.Value.Should().Be(1);
   }

   [Fact]
   public void Should_try_parse_string_with_custom_ISpanParsable_key()
   {
      var success = ValueObject_CustomSpanParsableKey.TryParse("2", null, out var result);
      success.Should().BeTrue();
      result.Should().NotBeNull();
      result!.Value.Value.Should().Be(2);
   }

   [Theory]
   [InlineData("en-US", "1234.56")]
   [InlineData("de-DE", "1234,56")]
   [InlineData("fr-FR", "1234,56")]
   public void Should_parse_span_decimal_with_culture_specific_separators(string cultureName, string input)
   {
      var culture = CultureInfo.CreateSpecificCulture(cultureName);
      var result = DecimalBasedClassValueObject.Parse(input.AsSpan(), culture);
      result.Property.Should().Be(1234.56m);
   }

   [Theory]
   [InlineData("en-US", "2.5")]
   [InlineData("de-DE", "2,5")]
   public void Should_try_parse_span_decimal_with_culture(string cultureName, string input)
   {
      var culture = CultureInfo.CreateSpecificCulture(cultureName);
      var success = DecimalBasedClassValueObject.TryParse(input.AsSpan(), culture, out var result);
      success.Should().BeTrue();
      result!.Property.Should().Be(2.5m);
   }

   [Theory]
   [InlineData("en-US", "invalid")]
   [InlineData("de-DE", "invalid")]
   public void Should_try_parse_span_return_false_with_invalid_decimal_for_culture(string cultureName, string input)
   {
      var culture = CultureInfo.CreateSpecificCulture(cultureName);
      var success = DecimalBasedClassValueObject.TryParse(input.AsSpan(), culture, out var result);
      success.Should().BeFalse();
      result.Should().BeNull();
   }

   [Theory]
   [InlineData("en-US", "01/01/2024")]
   [InlineData("de-DE", "01.01.2024")]
   public void Should_parse_span_datetime_with_culture_specific_formats(string cultureName, string input)
   {
      var culture = CultureInfo.CreateSpecificCulture(cultureName);
      var expected = TestValueObjectDateTime.Create(new DateTime(2024, 1, 1));
      var result = TestValueObjectDateTime.Parse(input.AsSpan(), culture);
      result.Should().Be(expected);
   }

   [Theory]
   [InlineData("en-US", "12/31/2024")]
   [InlineData("de-DE", "31.12.2024")]
   public void Should_try_parse_span_datetime_with_culture(string cultureName, string input)
   {
      var culture = CultureInfo.CreateSpecificCulture(cultureName);
      var expected = TestValueObjectDateTime.Create(new DateTime(2024, 12, 31));
      var success = TestValueObjectDateTime.TryParse(input.AsSpan(), culture, out var result);
      success.Should().BeTrue();
      result.Should().Be(expected);
   }

   [Fact]
   public void Should_parse_span_throw_FormatException_for_empty_span()
   {
      FluentActions.Invoking(() => IntBasedReferenceValueObject.Parse(ReadOnlySpan<char>.Empty, null))
                   .Should().Throw<FormatException>();
   }

   [Fact]
   public void Should_try_parse_span_return_false_for_empty_span()
   {
      var success = IntBasedReferenceValueObject.TryParse(ReadOnlySpan<char>.Empty, null, out var result);

      success.Should().BeFalse();
      result.Should().BeNull();
   }

   [Fact]
   public void Should_parse_span_throw_FormatException_for_invalid_malformed_decimal()
   {
      FluentActions.Invoking(() => DecimalBasedClassValueObject.Parse("12.34.56".AsSpan(), CultureInfo.InvariantCulture))
                   .Should().Throw<FormatException>();
   }

   [Fact]
   public void Should_try_parse_span_return_false_for_invalid_malformed_decimal()
   {
      var success = DecimalBasedClassValueObject.TryParse("12.34.56".AsSpan(), CultureInfo.InvariantCulture, out var result);

      success.Should().BeFalse();
      result.Should().BeNull();
   }

   [Fact]
   public void Should_parse_span_throw_FormatException_for_very_long_invalid_input()
   {
      var veryLongInvalid = new string('x', 1000);

      FluentActions.Invoking(() => IntBasedReferenceValueObject.Parse(veryLongInvalid.AsSpan(), null))
                   .Should().Throw<FormatException>();
   }

   [Fact]
   public void Should_try_parse_span_return_false_for_very_long_invalid_input()
   {
      var veryLongInvalid = new string('x', 1000);

      var success = IntBasedReferenceValueObject.TryParse(veryLongInvalid.AsSpan(), null, out var result);

      success.Should().BeFalse();
      result.Should().BeNull();
   }
#endif

   [Fact]
   public void Should_parse_throw_FormatException_for_malformed_decimal_multiple_separators()
   {
      var invalidDecimal = "12.34.56"; // Invalid format

      FluentActions.Invoking(() => DecimalBasedClassValueObject.Parse(invalidDecimal, CultureInfo.InvariantCulture))
                   .Should().Throw<FormatException>();
   }

   [Fact]
   public void Should_try_parse_return_false_for_malformed_decimal()
   {
      var invalidDecimal = "12.34.56";

      var success = DecimalBasedClassValueObject.TryParse(invalidDecimal, CultureInfo.InvariantCulture, out var result);

      success.Should().BeFalse();
      result.Should().BeNull();
   }

   [Fact]
   public void Should_parse_throw_FormatException_with_meaningful_message()
   {
      var invalidInput = "not_an_integer";

      FluentActions.Invoking(() => IntBasedReferenceValueObject.Parse(invalidInput, null))
                   .Should().Throw<FormatException>()
                   .WithMessage("The input string 'not_an_integer' was not in a correct format.");
   }

   [Fact]
   public void Should_try_parse_return_false_with_invalid_integer_format()
   {
      var invalidInput = "not_an_integer";

      var success = IntBasedReferenceValueObject.TryParse(invalidInput, null, out var result);

      success.Should().BeFalse();
      result.Should().BeNull();
   }
}

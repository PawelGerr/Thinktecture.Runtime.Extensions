using System;
using System.Globalization;
using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.ValueObjectTests;

public class TryParse
{
   [Fact]
   public void Should_parse_valid_value()
   {
      IntBasedReferenceValueObject.TryParse("1", null, out var intItem).Should().BeTrue();
      intItem.Should().Be(IntBasedReferenceValueObject.Create(1));

      IntBasedReferenceValueObject_With_StringBasedObjectFactory.TryParse("1", null, out var intItemWithStringFactory).Should().BeTrue();
      intItemWithStringFactory.Should().Be(IntBasedReferenceValueObject_With_StringBasedObjectFactory.Create(1));

      StringBasedReferenceValueObject.TryParse("test", null, out var stringItem).Should().BeTrue();
      stringItem.Should().Be(StringBasedReferenceValueObject.Create("test"));

      StringBasedReferenceValueObject_With_StringBasedObjectFactory.TryParse("test", null, out var stringItemWithFactory).Should().BeTrue();
      stringItemWithFactory.Should().Be(StringBasedReferenceValueObject_With_StringBasedObjectFactory.Create("test"));

#if NET9_0_OR_GREATER
      IntBasedReferenceValueObject_With_ReadOnlySpanOfCharBasedObjectFactory.TryParse("1", null, out var intItemWithSpanFactory).Should().BeTrue();
      intItemWithSpanFactory.Should().Be(IntBasedReferenceValueObject_With_ReadOnlySpanOfCharBasedObjectFactory.Create(1));

      IntBasedReferenceValueObject_With_StringAndReadOnlySpanOfCharBasedObjectFactory.TryParse("1", null, out var intItemWithBothFactories).Should().BeTrue();
      intItemWithBothFactories.Should().Be(IntBasedReferenceValueObject_With_StringAndReadOnlySpanOfCharBasedObjectFactory.Create(1));

      StringBasedReferenceValueObject_With_ReadOnlyBasedObjectFactory.TryParse("test", null, out var stringItemWithSpanFactory).Should().BeTrue();
      stringItemWithSpanFactory.Should().Be(StringBasedReferenceValueObject_With_ReadOnlyBasedObjectFactory.Create("test"));

      StringBasedReferenceValueObject_With_StringAndReadOnlyBasedObjectFactory.TryParse("test", null, out var stringItemWithBothFactories).Should().BeTrue();
      stringItemWithBothFactories.Should().Be(StringBasedReferenceValueObject_With_StringAndReadOnlyBasedObjectFactory.Create("test"));
#endif

      DecimalBasedClassValueObject.TryParse("1", null, out var item).Should().BeTrue();
      item.Should().Be(DecimalBasedClassValueObject.Create(1));

      DecimalBasedClassValueObject.TryParse("1.0", CultureInfo.InvariantCulture, out item).Should().BeTrue();
      item.Should().Be(DecimalBasedClassValueObject.Create(1));
   }

   [Fact]
   public void Should_parse_valid_value_having_custom_factory_method()
   {
      IntBasedReferenceValueObjectWithCustomFactoryNames.TryParse("1", null, out var item).Should().BeTrue();
      item.Should().Be(IntBasedReferenceValueObjectWithCustomFactoryNames.Get(1));
   }

   [Fact]
   public void Should_use_format_provider_parse_valid_value()
   {
      var german = CultureInfo.CreateSpecificCulture("de-DE");
      DecimalBasedClassValueObject.TryParse("1", german, out var item).Should().BeTrue();
      item.Should().Be(DecimalBasedClassValueObject.Create(1));

      DecimalBasedClassValueObject.TryParse("1,0", german, out item).Should().BeTrue();
      item.Should().Be(DecimalBasedClassValueObject.Create(1));
   }

   [Fact]
   public void Should_return_false_if_string_null()
   {
      DecimalBasedClassValueObject.TryParse(null, null, out var item).Should().BeFalse();
      item.Should().BeNull();
   }

   [Fact]
   public void Should_return_false_if_string_is_not_parsable_to_key_type()
   {
      DecimalBasedClassValueObject.TryParse(String.Empty, null, out var item).Should().BeFalse();
      item.Should().BeNull();

      DecimalBasedClassValueObject.TryParse("invalid", null, out item).Should().BeFalse();
      item.Should().BeNull();
   }

   [Fact]
   public void Should_support_TryParse_for_generic_string_based_value_objects()
   {
      var result = ValueObject_Generic_StringBased<object>.TryParse("test", null, out var obj);

      result.Should().BeTrue();
      obj.Should().NotBeNull();
      obj!.Value.Should().Be("test");
   }

#if NET9_0_OR_GREATER
   [Fact]
   public void Should_try_parse_ReadOnlySpanOfChar()
   {
      IntBasedReferenceValueObject.TryParse(1.ToString().AsSpan(), null, out var intItem).Should().BeTrue();
      intItem.Should().Be(IntBasedReferenceValueObject.Create(1));

      IntBasedReferenceValueObject_With_StringBasedObjectFactory.TryParse("1".AsSpan(), null, out var intItemWithStringFactory).Should().BeTrue();
      intItemWithStringFactory.Should().Be(IntBasedReferenceValueObject_With_StringBasedObjectFactory.Create(1));

      IntBasedReferenceValueObject_With_ReadOnlySpanOfCharBasedObjectFactory.TryParse("1".AsSpan(), null, out var intItemWithSpanFactory).Should().BeTrue();
      intItemWithSpanFactory.Should().Be(IntBasedReferenceValueObject_With_ReadOnlySpanOfCharBasedObjectFactory.Create(1));

      IntBasedReferenceValueObject_With_StringAndReadOnlySpanOfCharBasedObjectFactory.TryParse("1".AsSpan(), null, out var intItemWithBothFactories).Should().BeTrue();
      intItemWithBothFactories.Should().Be(IntBasedReferenceValueObject_With_StringAndReadOnlySpanOfCharBasedObjectFactory.Create(1));

      StringBasedReferenceValueObject.TryParse("test".AsSpan(), null, out var stringItem).Should().BeTrue();
      stringItem.Should().Be(StringBasedReferenceValueObject.Create("test"));

      StringBasedReferenceValueObject_With_StringBasedObjectFactory.TryParse("test".AsSpan(), null, out var stringItemWithFactory).Should().BeTrue();
      stringItemWithFactory.Should().Be(StringBasedReferenceValueObject_With_StringBasedObjectFactory.Create("test"));

      StringBasedReferenceValueObject_With_ReadOnlyBasedObjectFactory.TryParse("test".AsSpan(), null, out var stringItemWithSpanFactory).Should().BeTrue();
      stringItemWithSpanFactory.Should().Be(StringBasedReferenceValueObject_With_ReadOnlyBasedObjectFactory.Create("test"));

      StringBasedReferenceValueObject_With_StringAndReadOnlyBasedObjectFactory.TryParse("test".AsSpan(), null, out var stringItemWithBothFactories).Should().BeTrue();
      stringItemWithBothFactories.Should().Be(StringBasedReferenceValueObject_With_StringAndReadOnlyBasedObjectFactory.Create("test"));

      DecimalBasedClassValueObject.TryParse("1".AsSpan(), null, out var item).Should().BeTrue();
      item.Should().Be(DecimalBasedClassValueObject.Create(1));

      DecimalBasedClassValueObject.TryParse("1.0".AsSpan(), CultureInfo.InvariantCulture, out item).Should().BeTrue();
      item.Should().Be(DecimalBasedClassValueObject.Create(1));
   }
#endif

   [Theory]
   [InlineData("en-US", "1234.56")]
   [InlineData("de-DE", "1234,56")]
   [InlineData("fr-FR", "1234,56")]
   public void Should_try_parse_decimal_with_culture_specific_separators(string cultureName, string input)
   {
      var culture = CultureInfo.CreateSpecificCulture(cultureName);
      var success = DecimalBasedClassValueObject.TryParse(input, culture, out var result);
      success.Should().BeTrue();
      result!.Property.Should().Be(1234.56m);
   }

   [Fact]
   public void Should_try_parse_decimal_with_invariant_culture()
   {
      DecimalBasedClassValueObject.TryParse("2.5", CultureInfo.InvariantCulture, out var result).Should().BeTrue();
      result!.Property.Should().Be(2.5m);
   }

   [Fact]
   public void Should_try_parse_decimal_with_null_provider_uses_current_culture()
   {
      var originalCulture = CultureInfo.CurrentCulture;
      try
      {
         CultureInfo.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
         DecimalBasedClassValueObject.TryParse("1.5", null, out var result).Should().BeTrue();
         result!.Property.Should().Be(1.5m);
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
      var expected = TestValueObjectDateTime.Create(new DateTime(2024, 1, 1));
      var success = TestValueObjectDateTime.TryParse(input, culture, out var result);
      success.Should().BeTrue();
      result.Should().Be(expected);
   }

   [Fact]
   public void Should_try_parse_datetime_with_invariant_culture()
   {
      var expected = TestValueObjectDateTime.Create(new DateTime(2024, 1, 1));
      TestValueObjectDateTime.TryParse("01/01/2024", CultureInfo.InvariantCulture, out var result).Should().BeTrue();
      result.Should().Be(expected);
   }

#if NET9_0_OR_GREATER
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
   public void Should_try_parse_string_with_custom_ISpanParsable_key()
   {
      var success = ValueObject_CustomSpanParsableKey.TryParse("2", null, out var result);
      success.Should().BeTrue();
      result.Should().NotBeNull();
      result!.Value.Value.Should().Be(2);
   }

   [Fact]
   public void Should_try_parse_string_return_false_for_invalid_custom_ISpanParsable_key()
   {
      var success = ValueObject_CustomSpanParsableKey.TryParse("invalid", null, out var result);
      success.Should().BeFalse();
      result.Should().BeNull();
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
   public void Should_try_parse_span_return_false_for_empty_span()
   {
      var success = IntBasedReferenceValueObject.TryParse(ReadOnlySpan<char>.Empty, null, out var result);

      success.Should().BeFalse();
      result.Should().BeNull();
   }

   [Fact]
   public void Should_try_parse_span_return_false_for_invalid_malformed_decimal()
   {
      var success = DecimalBasedClassValueObject.TryParse("12.34.56".AsSpan(), CultureInfo.InvariantCulture, out var result);

      success.Should().BeFalse();
      result.Should().BeNull();
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
   public void Should_try_parse_return_false_for_malformed_decimal()
   {
      var invalidDecimal = "12.34.56";

      var success = DecimalBasedClassValueObject.TryParse(invalidDecimal, CultureInfo.InvariantCulture, out var result);

      success.Should().BeFalse();
      result.Should().BeNull();
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

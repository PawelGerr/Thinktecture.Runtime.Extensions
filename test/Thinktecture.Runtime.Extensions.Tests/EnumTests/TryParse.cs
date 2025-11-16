using System;
using System.Globalization;
using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTests;

public class TryParse
{
   [Fact]
   public void Should_parse_valid_value()
   {
      SmartEnum_DecimalBased.TryParse("1", null, out var item).Should().BeTrue();
      item.Should().Be(SmartEnum_DecimalBased.Value1);

      SmartEnum_DecimalBased.TryParse("1.0", CultureInfo.InvariantCulture, out item).Should().BeTrue();
      item.Should().Be(SmartEnum_DecimalBased.Value1);

      SmartEnum_CaseSensitive.TryParse("item", null, out var caseSensitiveItem).Should().BeTrue();
      caseSensitiveItem.Should().Be(SmartEnum_CaseSensitive.LowerCased);
   }

#if NET9_0_OR_GREATER
   [Fact]
   public void Should_parse_ReadOnlySpanOfChar()
   {
      SmartEnum_StringBased.TryParse(SmartEnum_StringBased.Item1.Key.AsSpan(), null, out var item).Should().BeTrue();
      item.Should().Be(SmartEnum_StringBased.Item1);
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
}

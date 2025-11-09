using System;
using System.Globalization;
using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTests;

public class Parse
{
   [Fact]
   public void Should_parse_valid_value()
   {
      SmartEnum_DecimalBased.Parse("1", null).Should().Be(SmartEnum_DecimalBased.Value1);

      SmartEnum_DecimalBased.Parse("1.0", CultureInfo.InvariantCulture).Should().Be(SmartEnum_DecimalBased.Value1);
   }

#if NET9_0_OR_GREATER
   [Fact]
   public void Should_parse_ReadOnlySpanOfChar()
   {
      SmartEnum_StringBased.Parse(SmartEnum_StringBased.Item1.Key.AsSpan(), null).Should().Be(SmartEnum_StringBased.Item1);
   }
#endif

   [Fact]
   public void Should_use_format_provider_parse_valid_value()
   {
      var german = CultureInfo.CreateSpecificCulture("de-DE");

      SmartEnum_DecimalBased.Parse("1", german).Should().Be(SmartEnum_DecimalBased.Value1);

      SmartEnum_DecimalBased.Parse("1,0", german).Should().Be(SmartEnum_DecimalBased.Value1);
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
}

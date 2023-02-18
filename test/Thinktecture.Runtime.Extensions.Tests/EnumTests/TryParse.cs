using System;
using System.Globalization;
using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTests;

public class TryParse
{
   [Fact]
   public void Should_parse_valid_value()
   {
      TestSmartEnum_Class_DecimalBased.TryParse("1", null, out var item).Should().BeTrue();
      item.Should().Be(TestSmartEnum_Class_DecimalBased.Value1);

      TestSmartEnum_Class_DecimalBased.TryParse("1.0", null, out item).Should().BeTrue();
      item.Should().Be(TestSmartEnum_Class_DecimalBased.Value1);
   }

   [Fact]
   public void Should_use_format_provider_parse_valid_value()
   {
      var german = CultureInfo.CreateSpecificCulture("de-DE");
      TestSmartEnum_Class_DecimalBased.TryParse("1", german, out var item).Should().BeTrue();
      item.Should().Be(TestSmartEnum_Class_DecimalBased.Value1);

      TestSmartEnum_Class_DecimalBased.TryParse("1,0", german, out item).Should().BeTrue();
      item.Should().Be(TestSmartEnum_Class_DecimalBased.Value1);
   }

   [Fact]
   public void Should_return_false_if_string_null()
   {
      TestSmartEnum_Class_DecimalBased.TryParse(null, null, out var item).Should().BeFalse();
      item.Should().BeNull();
   }

   [Fact]
   public void Should_return_false_if_string_is_not_parsable_to_key_type()
   {
      TestSmartEnum_Class_DecimalBased.TryParse(String.Empty, null, out var item).Should().BeFalse();
      item.Should().BeNull();

      TestSmartEnum_Class_DecimalBased.TryParse("invalid", null, out item).Should().BeFalse();
      item.Should().BeNull();
   }
}

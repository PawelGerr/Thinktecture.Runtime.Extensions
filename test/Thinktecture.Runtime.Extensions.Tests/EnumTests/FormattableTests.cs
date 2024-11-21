using System.Globalization;
using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTests;

public class FormattableTests
{
   [Fact]
   public void Should_use_format_string()
   {
      CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

      TestSmartEnum_Class_DecimalBased.Value1.ToString("P").Should().Be("100.00 %");
   }

   [Fact]
   public void Should_use_format_provider()
   {
      TestSmartEnum_Class_DecimalBased.Value1.ToString("P", CultureInfo.CreateSpecificCulture("de-DE")).Should().Be("100,000 %");
   }
}

using System.Globalization;
using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.ValueObjectTests;

public class FormattableTests
{
   [Fact]
   public void Should_use_format_string()
   {
      CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

      DecimalBasedClassValueObject.Create(1m).ToString("P").Should().Be("100.00 %");
   }

   [Fact]
   public void Should_use_format_provider()
   {
      DecimalBasedClassValueObject.Create(1m).ToString("P", CultureInfo.CreateSpecificCulture("de-DE")).Should().Be("100,000 %");
   }
}

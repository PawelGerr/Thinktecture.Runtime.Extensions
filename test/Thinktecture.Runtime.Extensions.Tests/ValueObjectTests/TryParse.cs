using System;
using System.Globalization;
using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.ValueObjectTests;

public class TryParse
{
   [Fact]
   public void Should_parse_valid_value()
   {
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
}

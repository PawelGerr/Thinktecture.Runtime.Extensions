using System;
using System.Globalization;
using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.ValueObjectTests;

public class Parse
{
   [Fact]
   public void Should_parse_valid_value()
   {
      DecimalBasedClassValueObject.Parse("1", null).Should().Be(DecimalBasedClassValueObject.Create(1));

      DecimalBasedClassValueObject.Parse("1.0", null).Should().Be(DecimalBasedClassValueObject.Create(1));
   }

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
}

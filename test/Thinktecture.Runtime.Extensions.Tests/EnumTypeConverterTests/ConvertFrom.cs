using System;
using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTypeConverterTests;

// ReSharper disable AssignNullToNotNullAttribute
// ReSharper disable PossibleNullReferenceException
public class ConvertFrom : TypeConverterTestsBase
{
   [Fact]
   public void Should_return_null_if_key_is_null()
   {
      SmartEnum_StringBased_TypeConverter.ConvertFrom(null, null, null).Should().BeNull();
   }

   [Fact]
   public void Should_return_item_if_parameter_matches_the_key_type_and_item_exists()
   {
      SmartEnum_StringBased_TypeConverter.ConvertFrom(null, null, "item1").Should().Be(SmartEnum_StringBased.Item1);
   }

   [Fact]
   public void Should_return_item_if_parameter_is_enum_already()
   {
      SmartEnum_StringBased_TypeConverter.ConvertFrom(null, null, SmartEnum_StringBased.Item1).Should().Be(SmartEnum_StringBased.Item1);
   }

   [Fact]
   public void Should_throw_if_parameter_type_doesnt_match_the_enum_and_key()
   {
      Action action = () => SmartEnum_StringBased_TypeConverter.ConvertFrom(Guid.Empty);
      action.Should().Throw<NotSupportedException>();
   }

   [Fact]
   public void Should_return_true_if_type_doesnt_matches_the_key_but_there_is_default_conversion_of_key_from_string()
   {
      SmartEnum_IntBased_TypeConverter.ConvertFrom(null, null, "1").Should().Be(SmartEnum_IntBased.Item1);
   }

   [Fact]
   public void Should_throw_trying_to_convert_unknown_key()
   {
      SmartEnum_StringBased_TypeConverter.Invoking(c => c.ConvertFrom(null, null, "invalid"))
                                         .Should().Throw<FormatException>().WithMessage("There is no item of type 'SmartEnum_StringBased' with the identifier 'invalid'.");
   }
}

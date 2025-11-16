using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTests;

public class ConversionToKey
{
   [Fact]
   public void Should_return_null_if_item_is_null()
   {
      string key = (SmartEnum_StringBased)null;

      key.Should().BeNull();
   }

   [Fact]
   public void Should_be_explicit_if_configured()
   {
      int key = (int)SmartEnum_CustomConversionOperators.Item;

      key.Should().Be(SmartEnum_CustomConversionOperators.Item.Key);
   }

   [Fact]
   public void Should_return_key()
   {
      string key = SmartEnum_StringBased.Item1;
      key.Should().Be(SmartEnum_StringBased.Item1.Key);
   }

   [Fact]
   public void Should_convert_to_int_implicitly_for_generic_int_based_enum()
   {
      int value = SmartEnum_Generic_IntBased<string>.Item1;
      value.Should().Be(1);

      value = SmartEnum_Generic_IntBased<string>.Item2;
      value.Should().Be(2);
   }

   [Fact]
   public void Should_convert_to_string_implicitly_for_generic_string_based_enum()
   {
      string value = SmartEnum_Generic_StringBased<int>.Item1;
      value.Should().Be("item1");

      value = SmartEnum_Generic_StringBased<int>.Item2;
      value.Should().Be("item2");
   }
}

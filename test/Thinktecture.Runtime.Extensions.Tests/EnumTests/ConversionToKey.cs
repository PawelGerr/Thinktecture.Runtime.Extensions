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
}

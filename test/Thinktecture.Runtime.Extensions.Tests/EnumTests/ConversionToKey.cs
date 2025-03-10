using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTests;

public class ConversionToKey
{
   [Fact]
   public void Should_return_null_if_item_is_null()
   {
      string key = (TestEnum)null;

      key.Should().BeNull();
   }

   [Fact]
   public void Should_be_explicit_if_configured()
   {
      int key = (int)TestEnumWithCustomConversionOperators.Item;

      key.Should().Be(TestEnumWithCustomConversionOperators.Item.Key);
   }

   [Fact]
   public void Should_return_default_if_struct_is_default()
   {
      StructIntegerEnum item = default;
      int key = item;

      key.Should().Be(0);
   }

   [Fact]
   public void Should_return_key()
   {
      string key = TestEnum.Item1;
      key.Should().Be(TestEnum.Item1.Key);
   }
}

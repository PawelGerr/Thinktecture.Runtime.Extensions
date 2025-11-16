using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTests;

public class CompareTo
{
   [Fact]
   public void Should_return_0_if_items_are_equal()
   {
      SmartEnum_DecimalBased.Value1.CompareTo(SmartEnum_DecimalBased.Value1).Should().Be(0);

      SmartEnum_CaseSensitive.LowerCased.CompareTo(SmartEnum_CaseSensitive.LowerCased).Should().Be(0);
   }

   [Fact]
   public void Should_return_1_if_items_is_bigger_than_other_item()
   {
      SmartEnum_DecimalBased.Value2.CompareTo(SmartEnum_DecimalBased.Value1).Should().Be(1);
      SmartEnum_DecimalBased.Value1.CompareTo(null).Should().Be(1);
      SmartEnum_DecimalBased.Value2.CompareTo(null).Should().Be(1);

      SmartEnum_CaseSensitive.LowerCased.CompareTo(SmartEnum_CaseSensitive.UpperCased).Should().Be(32);
      SmartEnum_CaseSensitive.LowerCased.CompareTo(null).Should().Be(1);
      SmartEnum_CaseSensitive.UpperCased.CompareTo(null).Should().Be(1);
   }

   [Fact]
   public void Should_return_minus_1_if_items_is_smaller_than_other_item()
   {
      SmartEnum_DecimalBased.Value1.CompareTo(SmartEnum_DecimalBased.Value2).Should().Be(-1);

      SmartEnum_CaseSensitive.UpperCased.CompareTo(SmartEnum_CaseSensitive.LowerCased).Should().Be(-32);
   }

   [Fact]
   public void Should_support_CompareTo_for_generic_int_based_enum()
   {
      var item1 = SmartEnum_Generic_IntBased<string>.Item1;
      var item2 = SmartEnum_Generic_IntBased<string>.Item2;

      item1.CompareTo(item2).Should().BeLessThan(0);
      item2.CompareTo(item1).Should().BeGreaterThan(0);
      item1.CompareTo(item1).Should().Be(0);
   }
}

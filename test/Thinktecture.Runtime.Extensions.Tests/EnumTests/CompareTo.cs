using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTests;

public class CompareTo
{
   [Fact]
   public void Should_return_0_if_items_are_equal()
   {
      TestSmartEnum_Class_DecimalBased.Value1.CompareTo(TestSmartEnum_Class_DecimalBased.Value1).Should().Be(0);

      ValidatableTestEnumCaseSensitive.LowerCased.CompareTo(ValidatableTestEnumCaseSensitive.LowerCased).Should().Be(0);
   }

   [Fact]
   public void Should_return_1_if_items_is_bigger_than_other_item()
   {
      TestSmartEnum_Class_DecimalBased.Value2.CompareTo(TestSmartEnum_Class_DecimalBased.Value1).Should().Be(1);
      TestSmartEnum_Class_DecimalBased.Value1.CompareTo(null).Should().Be(1);
      TestSmartEnum_Class_DecimalBased.Value2.CompareTo(null).Should().Be(1);

      ValidatableTestEnumCaseSensitive.LowerCased.CompareTo(ValidatableTestEnumCaseSensitive.UpperCased).Should().Be(32);
      ValidatableTestEnumCaseSensitive.LowerCased.CompareTo(null).Should().Be(1);
      ValidatableTestEnumCaseSensitive.UpperCased.CompareTo(null).Should().Be(1);
   }

   [Fact]
   public void Should_return_minus_1_if_items_is_smaller_than_other_item()
   {
      TestSmartEnum_Class_DecimalBased.Value1.CompareTo(TestSmartEnum_Class_DecimalBased.Value2).Should().Be(-1);

      ValidatableTestEnumCaseSensitive.UpperCased.CompareTo(ValidatableTestEnumCaseSensitive.LowerCased).Should().Be(-32);
   }
}

using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTests;

// ReSharper disable InconsistentNaming
public class ComparisonOperators
{
   [Fact]
   public void Should_compare_enums()
   {
      var item_1 = SmartEnum_IntBased.Item1;
      var item_2 = SmartEnum_IntBased.Item2;

      // ReSharper disable EqualExpressionComparison
      (item_1 < item_1).Should().BeFalse();
      (item_1 <= item_1).Should().BeTrue();
      (item_1 > item_1).Should().BeFalse();
      (item_1 >= item_1).Should().BeTrue();
      // ReSharper restore EqualExpressionComparison

      (item_1 < item_2).Should().BeTrue();
      (item_1 <= item_2).Should().BeTrue();
      (item_1 > item_2).Should().BeFalse();
      (item_1 >= item_2).Should().BeFalse();
   }

   [Fact]
   public void Should_compare_enums_with_case_sensitive_comparer()
   {
      var item_1 = SmartEnum_CaseSensitive.UpperCased;
      var item_2 = SmartEnum_CaseSensitive.LowerCased;

      // ReSharper disable EqualExpressionComparison
      (item_1 < item_1).Should().BeFalse();
      (item_1 <= item_1).Should().BeTrue();
      (item_1 > item_1).Should().BeFalse();
      (item_1 >= item_1).Should().BeTrue();
      // ReSharper restore EqualExpressionComparison

      (item_1 < item_2).Should().BeTrue();
      (item_1 <= item_2).Should().BeTrue();
      (item_1 > item_2).Should().BeFalse();
      (item_1 >= item_2).Should().BeFalse();
   }

   [Fact]
   public void Should_compare_enum_with_underlying_type()
   {
      var item_1 = SmartEnum_IntBased.Item1;
      var item_1_key = SmartEnum_IntBased.Item1.Key;
      var item_2 = SmartEnum_IntBased.Item2;
      var item_2_key = SmartEnum_IntBased.Item2.Key;

      (item_1 < item_1_key).Should().BeFalse();
      (item_1_key < item_1).Should().BeFalse();

      (item_1 <= item_1_key).Should().BeTrue();
      (item_1_key <= item_1).Should().BeTrue();

      (item_1 > item_1_key).Should().BeFalse();
      (item_1_key > item_1).Should().BeFalse();

      (item_1 >= item_1_key).Should().BeTrue();
      (item_1_key >= item_1).Should().BeTrue();

      (item_1_key < item_2).Should().BeTrue();
      (item_1 < item_2_key).Should().BeTrue();

      (item_1_key <= item_2).Should().BeTrue();
      (item_1 <= item_2_key).Should().BeTrue();

      (item_1_key > item_2).Should().BeFalse();
      (item_1 > item_2_key).Should().BeFalse();

      (item_1_key >= item_2).Should().BeFalse();
      (item_1 >= item_2_key).Should().BeFalse();
   }
}

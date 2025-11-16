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

   [Fact]
   public void Should_support_comparison_operators_for_generic_int_based_enum()
   {
      var item1 = SmartEnum_Generic_IntBased<string>.Item1;
      var item2 = SmartEnum_Generic_IntBased<string>.Item2;
      var item3 = SmartEnum_Generic_IntBased<string>.Item3;

      (item1 < item2).Should().BeTrue();
      (item2 < item3).Should().BeTrue();
      (item1 < item3).Should().BeTrue();

      (item2 > item1).Should().BeTrue();
      (item3 > item2).Should().BeTrue();
      (item3 > item1).Should().BeTrue();

      (item1 <= item1).Should().BeTrue();
      (item1 <= item2).Should().BeTrue();
      (item2 >= item2).Should().BeTrue();
      (item3 >= item1).Should().BeTrue();
   }

   [Fact]
   public void Should_support_comparison_operators_for_generic_string_based_enum()
   {
      var item1 = SmartEnum_Generic_StringBased<int>.Item1;
      var item2 = SmartEnum_Generic_StringBased<int>.Item2;

      (item1 < item2).Should().BeTrue();
      (item2 > item1).Should().BeTrue();
      (item1 <= item1).Should().BeTrue();
      (item2 >= item2).Should().BeTrue();
   }
}

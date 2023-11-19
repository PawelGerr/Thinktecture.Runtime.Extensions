using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTests;

// ReSharper disable SuspiciousTypeConversion.Global
// ReSharper disable EqualExpressionComparison
// ReSharper disable ConditionIsAlwaysTrueOrFalse
public class EqualityOperator
{
   [Fact]
   public void Should_return_false_if_item_is_null()
   {
      (TestEnum.Item1 is null).Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_if_item_is_of_different_type()
   {
      (TestEnum.Item1 == TestEnumWithNonDefaultComparer.Item).Should().BeFalse();
   }

   [Fact]
   public void Should_return_true_on_reference_equality()
   {
      (TestEnum.Item1 == TestEnum.Item1).Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_on_struct_equality()
   {
      // ReSharper disable once EqualExpressionComparison
      (StructIntegerEnum.Item1 == StructIntegerEnum.Item1).Should().BeTrue();

      // ReSharper disable once EqualExpressionComparison
      (StructIntegerEnumWithZero.Item0 == StructIntegerEnumWithZero.Item0).Should().BeTrue();
   }

   [Fact]
   public void Should_return_false_on_struct_inequality()
   {
      // ReSharper disable once EqualExpressionComparison
      (StructIntegerEnum.Item1 == StructIntegerEnum.Item2).Should().BeFalse();

      (StructIntegerEnumWithZero.Item0 == new StructIntegerEnumWithZero()).Should().BeFalse();
   }

   [Fact]
   public void Should_return_true_for_invalid_structs_on_equality()
   {
      // ReSharper disable once EqualExpressionComparison
      (StructIntegerEnum.Get(42) == StructIntegerEnum.Get(42)).Should().BeTrue();

      // ReSharper disable once EqualExpressionComparison
      (new StructIntegerEnumWithZero() == new StructIntegerEnumWithZero()).Should().BeTrue();
   }

   [Fact]
   public void Should_return_false_for_invalid_structs_on_inequality()
   {
      // ReSharper disable once EqualExpressionComparison
      (StructIntegerEnum.Get(42) == StructIntegerEnum.Get(43)).Should().BeFalse();
   }

   [Fact]
   public void Should_return_true_if_both_items_are_invalid_and_have_same_key()
   {
      (TestEnum.Get("unknown") == TestEnum.Get("Unknown")).Should().BeTrue();
   }

   [Fact]
   public void Should_return_false_if_both_items_are_invalid_and_have_different_keys()
   {
      (TestEnum.Get("unknown") == TestEnum.Get("other")).Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_if_both_items_are_invalid_and_have_keys_that_differ_in_casing_if_comparer_honors_casing()
   {
      (TestEnumWithNonDefaultComparer.Get("Item") == TestEnumWithNonDefaultComparer.Get("item")).Should().BeFalse();
   }

   [Fact]
   public void Should_compare_keyless_smart_enum_via_reference_equality()
   {
      (KeylessTestEnum.Item1 == KeylessTestEnum.Item1).Should().BeTrue();
      (KeylessTestEnum.Item2 == KeylessTestEnum.Item2).Should().BeTrue();
      (KeylessTestEnum.Item1 == KeylessTestEnum.Item2).Should().BeFalse();
   }
}

using Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable ConditionIsAlwaysTrueOrFalse
// ReSharper disable EqualExpressionComparison

namespace Thinktecture.Runtime.Tests.EnumTests;

public class NotEqualityOperator
{
   [Fact]
   public void Should_return_true_if_item_is_null()
   {
      (TestEnum.Item1 is not null).Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_if_item_is_of_different_type()
   {
      // ReSharper disable once SuspiciousTypeConversion.Global
      (TestEnum.Item1 != ValidatableTestEnumCaseSensitive.LowerCased).Should().BeTrue();
   }

   [Fact]
   public void Should_return_false_on_reference_equality()
   {
      // ReSharper disable once EqualExpressionComparison
      (TestEnum.Item1 != TestEnum.Item1).Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_if_both_items_are_invalid_and_have_same_key()
   {
      (TestEnum.Get("unknown") != TestEnum.Get("Unknown")).Should().BeFalse();
   }

   [Fact]
   public void Should_return_true_if_both_items_are_invalid_and_have_different_keys()
   {
      (TestEnum.Get("unknown") != TestEnum.Get("other")).Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_if_both_items_are_invalid_and_have_keys_that_differ_in_casing_if_comparer_honors_casing()
   {
      (ValidatableTestEnumCaseSensitive.Get("INVALID") != ValidatableTestEnumCaseSensitive.Get("invalid")).Should().BeTrue();
   }

   [Fact]
   public void Should_compare_keyless_smart_enum_via_reference_equality()
   {
      (KeylessTestEnum.Item1 != KeylessTestEnum.Item1).Should().BeFalse();
      (KeylessTestEnum.Item2 != KeylessTestEnum.Item2).Should().BeFalse();
      (KeylessTestEnum.Item1 != KeylessTestEnum.Item2).Should().BeTrue();
   }
}

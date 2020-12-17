using FluentAssertions;
using Thinktecture.TestEnums;
using Xunit;

namespace Thinktecture.EnumTests
{
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
         // ReSharper disable once SuspiciousTypeConversion.Global
         (TestEnum.Item1 == TestEnumWithNonDefaultComparer.Item).Should().BeFalse();
      }

      [Fact]
      public void Should_return_true_on_reference_equality()
      {
         // ReSharper disable once EqualExpressionComparison
         (TestEnum.Item1 == TestEnum.Item1).Should().BeTrue();
      }

      [Fact]
      public void Should_return_true_on_struct_equality()
      {
         // ReSharper disable once EqualExpressionComparison
         (StructIntegerEnum.Item1 == StructIntegerEnum.Item1).Should().BeTrue();
      }

      [Fact]
      public void Should_return_false_on_struct_inequality()
      {
         // ReSharper disable once EqualExpressionComparison
         (StructIntegerEnum.Item1 == StructIntegerEnum.Item2).Should().BeFalse();
      }

      [Fact]
      public void Should_return_true_for_invalid__structs_on_equality()
      {
         // ReSharper disable once EqualExpressionComparison
         (StructIntegerEnum.Get(42) == StructIntegerEnum.Get(42)).Should().BeTrue();
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
   }
}

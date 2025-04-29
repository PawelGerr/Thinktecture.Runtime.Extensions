using System;
using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTests;

public class TryGet
{
   [Fact]
   public void Should_return_false_if_null_is_provided()
   {
      SmartEnum_Empty.TryGet(null, out var item).Should().BeFalse();
      item.Should().BeNull();
   }

   [Fact]
   public void Should_return_false_if_enum_dont_have_item_with_provided_key()
   {
      SmartEnum_StringBased.TryGet("unknown", out var item).Should().BeFalse();
      item.Should().BeNull();
   }

#if NET9_0_OR_GREATER
   [Fact]
   public void Should_return_item_having_ReadOnlySpanOfChar()
   {
      SmartEnum_StringBased.TryGet(SmartEnum_StringBased.Item1.Key.AsSpan(), out var item).Should().BeTrue();
      item.Should().Be(SmartEnum_StringBased.Item1);
   }
#endif

   [Fact]
   public void Should_return_true_if_item_with_provided_key_exists()
   {
      SmartEnum_StringBased.TryGet("item2", out var item).Should().BeTrue();
      item.Should().Be(SmartEnum_StringBased.Item2);
   }

   [Fact]
   public void Should_return_true_if_item_with_provided_key_exists_ignoring_casing()
   {
      SmartEnum_StringBased.TryGet("Item1", out var item).Should().BeTrue();
      item.Should().Be(SmartEnum_StringBased.Item1);

      SmartEnum_StringBased.TryGet("item1", out item).Should().BeTrue();
      item.Should().Be(SmartEnum_StringBased.Item1);
   }

   [Fact]
   public void Should_return_item_of_case_sensitive_enum()
   {
      SmartEnum_CaseSensitive.TryGet("item", out var item).Should().BeTrue();
      item.Should().Be(SmartEnum_CaseSensitive.LowerCased);

      SmartEnum_CaseSensitive.TryGet("ITEM", out item).Should().BeTrue();
      item.Should().Be(SmartEnum_CaseSensitive.UpperCased);
   }

   [Fact]
   public void Should_return_false_if_the_casing_does_not_match_according_to_comparer()
   {
      SmartEnum_CaseSensitive.TryGet("ITEM2", out var item).Should().BeFalse();
      item.Should().BeNull();
   }
}

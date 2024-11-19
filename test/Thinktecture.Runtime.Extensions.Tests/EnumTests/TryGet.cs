using System;
using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTests;

public class TryGet
{
   [Fact]
   public void Should_return_false_if_null_is_provided()
   {
      EmptyEnum.TryGet(null, out var item).Should().BeFalse();
      item.Should().BeNull();
   }

   [Fact]
   public void Should_return_false_if_enum_dont_have_any_items()
   {
      EmptyEnum.TryGet("unknown", out var item).Should().BeFalse();
      item.Should().NotBeNull();
      item!.IsValid.Should().BeFalse();
      item!.Key.Should().Be("unknown");
   }

   [Fact]
   public void Should_return_false_if_struct_dont_have_any_items()
   {
      StructIntegerEnum.TryGet(42, out var item).Should().BeFalse();
      item.IsValid.Should().BeFalse();
      item.Key.Should().Be(42);
   }

   [Fact]
   public void Should_return_false_if_enum_dont_have_item_with_provided_key()
   {
      TestEnum.TryGet("unknown", out var item).Should().BeFalse();
      item.Should().NotBeNull();
      item!.IsValid.Should().BeFalse();
      item!.Key.Should().Be("unknown");
   }

#if NET9_0_OR_GREATER
   [Fact]
   public void Should_return_item_having_ReadOnlySpanOfChar()
   {
      TestEnum.TryGet(TestEnum.Item1.Key.AsSpan(), out var item).Should().BeTrue();
      item.Should().Be(TestEnum.Item1);
   }
#endif

   [Fact]
   public void Should_return_true_if_item_with_provided_key_exists()
   {
      TestEnum.TryGet("item2", out var item).Should().BeTrue();
      item.Should().Be(TestEnum.Item2);
   }

   [Fact]
   public void Should_return_true_if_item_with_provided_key_exists_ignoring_casing()
   {
      TestEnum.TryGet("Item1", out var item).Should().BeTrue();
      item.Should().Be(TestEnum.Item1);

      TestEnum.TryGet("item1", out item).Should().BeTrue();
      item.Should().Be(TestEnum.Item1);
   }

   [Fact]
   public void Should_return_item_of_case_sensitive_enum()
   {
      ValidatableTestEnumCaseSensitive.TryGet("item", out var item).Should().BeTrue();
      item.Should().Be(ValidatableTestEnumCaseSensitive.LowerCased);

      ValidatableTestEnumCaseSensitive.TryGet("ITEM", out item).Should().BeTrue();
      item.Should().Be(ValidatableTestEnumCaseSensitive.UpperCased);
   }

   [Fact]
   public void Should_return_false_if_the_casing_does_not_match_according_to_comparer()
   {
      ValidatableTestEnumCaseSensitive.TryGet("ITEM2", out var item).Should().BeFalse();
      item.Should().NotBeNull();
      item!.IsValid.Should().BeFalse();
      item!.Key.Should().Be("ITEM2");
   }
}

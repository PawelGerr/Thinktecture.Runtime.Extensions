using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTests;

public class TryGet
{
   [Fact]
   public void Should_return_false_if_null_is_provided()
   {
      EmptyEnum.TryGet(null, out var item).Should().BeFalse();
      item.Should().BeNull();

      ExtensibleTestEnum.TryGet(null, out var extensibleItem).Should().BeFalse();
      extensibleItem.Should().BeNull();

      ExtendedTestEnum.TryGet(null, out var extendedItem).Should().BeFalse();
      extendedItem.Should().BeNull();

      DifferentAssemblyExtendedTestEnum.TryGet(null, out var differentAssemblyExtendedItem).Should().BeFalse();
      differentAssemblyExtendedItem.Should().BeNull();
   }

   [Fact]
   public void Should_return_false_if_enum_dont_have_any_items()
   {
      EmptyEnum.TryGet("unknown", out var item).Should().BeFalse();
      item.Should().BeNull();
   }

   [Fact]
   public void Should_return_false_if_struct_dont_have_any_items()
   {
      StructIntegerEnum.TryGet(42, out var item).Should().BeFalse();
      item.Should().Be(new StructIntegerEnum());
   }

   [Fact]
   public void Should_return_false_if_enum_dont_have_item_with_provided_key()
   {
      TestEnum.TryGet("unknown", out var item).Should().BeFalse();
      item.Should().BeNull();

      ExtensibleTestEnum.TryGet("unknown", out var extensibleItem).Should().BeFalse();
      extensibleItem.Should().BeNull();

      ExtendedTestEnum.TryGet("unknown", out var extendedItem).Should().BeFalse();
      extendedItem.Should().BeNull();

      DifferentAssemblyExtendedTestEnum.TryGet("unknown", out var differentAssemblyExtendedItem).Should().BeFalse();
      differentAssemblyExtendedItem.Should().BeNull();
   }

   [Fact]
   public void Should_return_true_if_item_with_provided_key_exists()
   {
      TestEnum.TryGet("item2", out var item).Should().BeTrue();
      item.Should().Be(TestEnum.Item2);

      ExtensibleTestEnum.TryGet("Item1", out var extensibleItem).Should().BeTrue();
      extensibleItem.Should().Be(ExtensibleTestEnum.Item1);

      ExtendedTestEnum.TryGet("Item1", out var extendedItem).Should().BeTrue();
      extendedItem.Should().Be(ExtendedTestEnum.Item1);

      ExtendedTestEnum.TryGet("Item2", out extendedItem).Should().BeTrue();
      extendedItem.Should().Be(ExtendedTestEnum.Item2);

      DifferentAssemblyExtendedTestEnum.TryGet("Item1", out var differentAssemblyExtendedItem).Should().BeTrue();
      differentAssemblyExtendedItem.Should().Be(DifferentAssemblyExtendedTestEnum.Item1);

      DifferentAssemblyExtendedTestEnum.TryGet("Item2", out differentAssemblyExtendedItem).Should().BeTrue();
      differentAssemblyExtendedItem.Should().Be(DifferentAssemblyExtendedTestEnum.Item2);
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
   public void Should_return_false_if_the_casing_does_not_match_according_to_comparer()
   {
      TestEnumWithNonDefaultComparer.TryGet("Item2", out var item).Should().BeFalse();
      item.Should().BeNull();
   }
}
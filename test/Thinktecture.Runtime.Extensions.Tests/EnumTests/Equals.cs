using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTests;

// ReSharper disable SuspiciousTypeConversion.Global
public class Equals
{
   [Fact]
   public void Should_return_false_if_item_is_null()
   {
      TestEnum.Item1.Equals(null).Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_if_item_is_of_different_type()
   {
      TestEnum.Item1.Equals(TestEnumWithNonDefaultComparer.Item).Should().BeFalse();
   }

   [Fact]
   public void Should_return_true_on_reference_equality()
   {
      TestEnum.Item1.Equals(TestEnum.Item1).Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_if_both_items_are_invalid_and_have_same_key()
   {
      TestEnum.Get("unknown").Equals(TestEnum.Get("Unknown")).Should().BeTrue();
   }

   [Fact]
   public void Should_return_false_if_both_items_are_invalid_and_have_different_keys()
   {
      TestEnum.Get("unknown").Equals(TestEnum.Get("other")).Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_if_both_items_are_invalid_and_have_keys_that_differ_in_casing_if_comparer_honors_casing()
   {
      TestEnumWithNonDefaultComparer.Get("Item").Equals(TestEnumWithNonDefaultComparer.Get("item")).Should().BeFalse();
   }

   [Fact]
   public void Should_compare_keyless_smart_enum_via_reference_equality()
   {
      KeylessTestEnum.Item1.Equals(KeylessTestEnum.Item1).Should().BeTrue();
      KeylessTestEnum.Item2.Equals(KeylessTestEnum.Item2).Should().BeTrue();
      KeylessTestEnum.Item1.Equals(KeylessTestEnum.Item2).Should().BeFalse();
   }
}

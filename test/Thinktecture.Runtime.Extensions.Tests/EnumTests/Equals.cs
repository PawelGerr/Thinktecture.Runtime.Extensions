using FluentAssertions;
using Thinktecture.Runtime.Tests.TestEnums;
using Xunit;

namespace Thinktecture.Runtime.Tests.EnumTests
{
   public class Equals
   {
      [Fact]
      public void Should_return_false_if_item_is_null()
      {
         TestEnum.Item1.Equals(null).Should().BeFalse();

         ExtensibleTestEnum.Item1.Equals(null).Should().BeFalse();
         ExtensibleTestEnum.DerivedItem.Equals(null).Should().BeFalse();
         ExtendedTestEnum.Item1.Equals(null).Should().BeFalse();
         ExtendedTestEnum.DerivedItem.Equals(null).Should().BeFalse();
         ExtensibleTestValidatableEnum.Item1.Equals(null).Should().BeFalse();
         ExtendedTestValidatableEnum.Item1.Equals(null).Should().BeFalse();
      }

      [Fact]
      public void Should_return_false_if_item_is_of_different_type()
      {
         // ReSharper disable once SuspiciousTypeConversion.Global
         TestEnum.Item1.Equals(TestEnumWithNonDefaultComparer.Item).Should().BeFalse();

         ExtensibleTestEnum.Item1.Equals(ExtendedTestEnum.Item1).Should().BeFalse();
         ExtensibleTestEnum.DerivedItem.Equals(ExtendedTestEnum.DerivedItem).Should().BeFalse();
         ExtensibleTestValidatableEnum.Item1.Equals(ExtendedTestValidatableEnum.Item1).Should().BeFalse();
      }

      [Fact]
      public void Should_return_true_on_reference_equality()
      {
         TestEnum.Item1.Equals(TestEnum.Item1).Should().BeTrue();

         ExtensibleTestEnum.Item1.Equals(ExtensibleTestEnum.Item1).Should().BeTrue();
         ExtensibleTestEnum.DerivedItem.Equals(ExtensibleTestEnum.DerivedItem).Should().BeTrue();
         ExtendedTestEnum.Item1.Equals(ExtendedTestEnum.Item1).Should().BeTrue();
         ExtendedTestEnum.Item2.Equals(ExtendedTestEnum.Item2).Should().BeTrue();
         ExtendedTestEnum.DerivedItem.Equals(ExtendedTestEnum.DerivedItem).Should().BeTrue();
      }

      [Fact]
      public void Should_return_true_if_both_items_are_invalid_and_have_same_key()
      {
         TestEnum.Get("unknown").Equals(TestEnum.Get("Unknown")).Should().BeTrue();

         ExtensibleTestValidatableEnum.Get("unknown").Equals(ExtensibleTestValidatableEnum.Get("Unknown")).Should().BeTrue();
         ExtendedTestValidatableEnum.Get("unknown").Equals(ExtendedTestValidatableEnum.Get("Unknown")).Should().BeTrue();
      }

      [Fact]
      public void Should_return_false_if_both_items_are_invalid_and_have_different_keys()
      {
         TestEnum.Get("unknown").Equals(TestEnum.Get("other")).Should().BeFalse();

         ExtensibleTestValidatableEnum.Get("unknown").Equals(ExtensibleTestValidatableEnum.Get("other")).Should().BeFalse();
         ExtendedTestValidatableEnum.Get("unknown").Equals(ExtendedTestValidatableEnum.Get("other")).Should().BeFalse();
      }

      [Fact]
      public void Should_return_false_if_both_items_are_invalid_and_have_keys_that_differ_in_casing_if_comparer_honors_casing()
      {
         TestEnumWithNonDefaultComparer.Get("Item").Equals(TestEnumWithNonDefaultComparer.Get("item")).Should().BeFalse();
      }
   }
}

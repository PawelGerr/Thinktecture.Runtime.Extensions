using FluentAssertions;
using Thinktecture.Runtime.Tests.TestEnums;
using Xunit;

// ReSharper disable ConditionIsAlwaysTrueOrFalse
// ReSharper disable EqualExpressionComparison

namespace Thinktecture.Runtime.Tests.EnumTests
{
   public class NotEqualityOperator
   {
      [Fact]
      public void Should_return_true_if_item_is_null()
      {
         (TestEnum.Item1 is not null).Should().BeTrue();

         (ExtensibleTestEnum.Item1 is not null).Should().BeTrue();
         (ExtendedTestEnum.DerivedItem is not null).Should().BeTrue();
         (ExtendedTestEnum.Item1 is not null).Should().BeTrue();
         (ExtendedTestEnum.Item2 is not null).Should().BeTrue();
         (DifferentAssemblyExtendedTestEnum.DerivedItem is not null).Should().BeTrue();
         (DifferentAssemblyExtendedTestEnum.Item1 is not null).Should().BeTrue();
         (DifferentAssemblyExtendedTestEnum.Item2 is not null).Should().BeTrue();
      }

      [Fact]
      public void Should_return_true_if_item_is_of_different_type()
      {
         // ReSharper disable once SuspiciousTypeConversion.Global
         (TestEnum.Item1 != TestEnumWithNonDefaultComparer.Item).Should().BeTrue();

         (ExtensibleTestEnum.Item1 != ExtendedTestEnum.Item1).Should().BeTrue();
         (ExtensibleTestEnum.DerivedItem != ExtendedTestEnum.DerivedItem).Should().BeTrue();
         (ExtensibleTestEnum.Item1 != DifferentAssemblyExtendedTestEnum.Item1).Should().BeTrue();
         (ExtensibleTestEnum.DerivedItem != DifferentAssemblyExtendedTestEnum.DerivedItem).Should().BeTrue();
         (ExtensibleTestValidatableEnum.Item1 != ExtendedTestValidatableEnum.Item1).Should().BeTrue();
      }

      [Fact]
      public void Should_return_false_on_reference_equality()
      {
         // ReSharper disable once EqualExpressionComparison
         (TestEnum.Item1 != TestEnum.Item1).Should().BeFalse();

         (ExtensibleTestEnum.Item1 != ExtensibleTestEnum.Item1).Should().BeFalse();
         (ExtensibleTestEnum.DerivedItem != ExtensibleTestEnum.DerivedItem).Should().BeFalse();
         (ExtendedTestEnum.Item1 != ExtendedTestEnum.Item1).Should().BeFalse();
         (ExtendedTestEnum.Item2 != ExtendedTestEnum.Item2).Should().BeFalse();
         (ExtendedTestEnum.DerivedItem != ExtendedTestEnum.DerivedItem).Should().BeFalse();
         (DifferentAssemblyExtendedTestEnum.Item1 != DifferentAssemblyExtendedTestEnum.Item1).Should().BeFalse();
         (DifferentAssemblyExtendedTestEnum.Item2 != DifferentAssemblyExtendedTestEnum.Item2).Should().BeFalse();
         (DifferentAssemblyExtendedTestEnum.DerivedItem != DifferentAssemblyExtendedTestEnum.DerivedItem).Should().BeFalse();
      }

      [Fact]
      public void Should_return_false_if_both_items_are_invalid_and_have_same_key()
      {
         (TestEnum.Get("unknown") != TestEnum.Get("Unknown")).Should().BeFalse();

         (ExtensibleTestValidatableEnum.Get("unknown") != ExtensibleTestValidatableEnum.Get("Unknown")).Should().BeFalse();
         (ExtendedTestValidatableEnum.Get("unknown") != ExtendedTestValidatableEnum.Get("Unknown")).Should().BeFalse();
      }

      [Fact]
      public void Should_return_true_if_both_items_are_invalid_and_have_different_keys()
      {
         (TestEnum.Get("unknown") != TestEnum.Get("other")).Should().BeTrue();

         (ExtensibleTestValidatableEnum.Get("unknown") != ExtensibleTestValidatableEnum.Get("other")).Should().BeTrue();
         (ExtendedTestValidatableEnum.Get("unknown") != ExtendedTestValidatableEnum.Get("other")).Should().BeTrue();
      }

      [Fact]
      public void Should_return_true_if_both_items_are_invalid_and_have_keys_that_differ_in_casing_if_comparer_honors_casing()
      {
         (TestEnumWithNonDefaultComparer.Get("Item") != TestEnumWithNonDefaultComparer.Get("item")).Should().BeTrue();
      }
   }
}

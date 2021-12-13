using FluentAssertions;
using Thinktecture.Runtime.Tests.TestEnums;
using Xunit;

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

      (ExtensibleTestEnum.Item1 is null).Should().BeFalse();
      (ExtendedTestEnum.DerivedItem is null).Should().BeFalse();
      (ExtendedTestEnum.Item1 is null).Should().BeFalse();
      (ExtendedTestEnum.Item2 is null).Should().BeFalse();
      (DifferentAssemblyExtendedTestEnum.DerivedItem is null).Should().BeFalse();
      (DifferentAssemblyExtendedTestEnum.Item1 is null).Should().BeFalse();
      (DifferentAssemblyExtendedTestEnum.Item2 is null).Should().BeFalse();
   }

   [Fact]
   public void Should_return_true_if_item_is_extended_clone()
   {
      // equality should be symmetrical

      (ExtensibleTestEnum.Item1 == ExtendedTestEnum.Item1).Should().BeTrue();
      (ExtensibleTestEnum.DerivedItem == ExtendedTestEnum.DerivedItem).Should().BeTrue();
      (ExtensibleTestEnum.Item1 == DifferentAssemblyExtendedTestEnum.Item1).Should().BeTrue();
      (ExtensibleTestEnum.DerivedItem == DifferentAssemblyExtendedTestEnum.DerivedItem).Should().BeTrue();
      (ExtensibleTestEnum.Item1 == ExtendedSiblingTestEnum.Item1).Should().BeTrue();
      (ExtensibleTestEnum.DerivedItem == ExtendedSiblingTestEnum.DerivedItem).Should().BeTrue();
      (ExtensibleTestValidatableEnum.Item1 == ExtendedTestValidatableEnum.Item1).Should().BeTrue();

      (ExtendedTestEnum.Item1 == ExtensibleTestEnum.Item1).Should().BeTrue();
      (ExtendedTestEnum.DerivedItem == ExtensibleTestEnum.DerivedItem).Should().BeTrue();
      (DifferentAssemblyExtendedTestEnum.Item1 == ExtensibleTestEnum.Item1).Should().BeTrue();
      (DifferentAssemblyExtendedTestEnum.DerivedItem == ExtensibleTestEnum.DerivedItem).Should().BeTrue();
      (ExtendedSiblingTestEnum.Item1 == ExtensibleTestEnum.Item1).Should().BeTrue();
      (ExtendedSiblingTestEnum.DerivedItem == ExtensibleTestEnum.DerivedItem).Should().BeTrue();
      (ExtendedTestValidatableEnum.Item1 == ExtensibleTestValidatableEnum.Item1).Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_if_comparing_base_items_of_extended_clone_siblings()
   {
      (ExtendedTestEnum.Item1 == ExtendedSiblingTestEnum.Item1).Should().BeTrue();
      (ExtendedSiblingTestEnum.Item1 == ExtendedTestEnum.Item1).Should().BeTrue();
   }

   [Fact]
   public void Should_return_false_if_items_are_extended_clone_siblings_having_same_key()
   {
      (ExtendedTestEnum.Item2 == ExtendedSiblingTestEnum.Item2).Should().BeFalse();
      (ExtendedSiblingTestEnum.Item2 == ExtendedTestEnum.Item2).Should().BeFalse();
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

      (ExtensibleTestEnum.Item1 == ExtensibleTestEnum.Item1).Should().BeTrue();
      (ExtensibleTestEnum.DerivedItem == ExtensibleTestEnum.DerivedItem).Should().BeTrue();
      (ExtendedTestEnum.Item1 == ExtendedTestEnum.Item1).Should().BeTrue();
      (ExtendedTestEnum.Item2 == ExtendedTestEnum.Item2).Should().BeTrue();
      (ExtendedTestEnum.DerivedItem == ExtendedTestEnum.DerivedItem).Should().BeTrue();
      (DifferentAssemblyExtendedTestEnum.Item1 == DifferentAssemblyExtendedTestEnum.Item1).Should().BeTrue();
      (DifferentAssemblyExtendedTestEnum.Item2 == DifferentAssemblyExtendedTestEnum.Item2).Should().BeTrue();
      (DifferentAssemblyExtendedTestEnum.DerivedItem == DifferentAssemblyExtendedTestEnum.DerivedItem).Should().BeTrue();
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

      (ExtensibleTestValidatableEnum.Get("unknown") == ExtensibleTestValidatableEnum.Get("Unknown")).Should().BeTrue();
      (ExtendedTestValidatableEnum.Get("unknown") == ExtendedTestValidatableEnum.Get("Unknown")).Should().BeTrue();
   }

   [Fact]
   public void Should_return_false_if_both_items_are_invalid_and_have_different_keys()
   {
      (TestEnum.Get("unknown") == TestEnum.Get("other")).Should().BeFalse();

      (ExtensibleTestValidatableEnum.Get("unknown") == ExtensibleTestValidatableEnum.Get("other")).Should().BeFalse();
      (ExtendedTestValidatableEnum.Get("unknown") == ExtendedTestValidatableEnum.Get("other")).Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_if_both_items_are_invalid_and_have_keys_that_differ_in_casing_if_comparer_honors_casing()
   {
      (TestEnumWithNonDefaultComparer.Get("Item") == TestEnumWithNonDefaultComparer.Get("item")).Should().BeFalse();
   }
}
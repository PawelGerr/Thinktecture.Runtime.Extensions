using Thinktecture.CodeAnalysis;

#nullable enable

namespace Thinktecture.Runtime.Tests.ImplementedComparisonOperatorsExtensionsTests;

public class HasOperator
{
   [Fact]
   public void Should_return_false_when_checking_for_None()
   {
      var operators = ImplementedComparisonOperators.GreaterThan;

      var result = operators.HasOperator(ImplementedComparisonOperators.None);

      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_when_both_are_None()
   {
      var operators = ImplementedComparisonOperators.None;

      var result = operators.HasOperator(ImplementedComparisonOperators.None);

      result.Should().BeFalse();
   }

   [Theory]
   [InlineData(ImplementedComparisonOperators.GreaterThan, ImplementedComparisonOperators.GreaterThan, true)]
   [InlineData(ImplementedComparisonOperators.GreaterThanOrEqual, ImplementedComparisonOperators.GreaterThanOrEqual, true)]
   [InlineData(ImplementedComparisonOperators.LessThan, ImplementedComparisonOperators.LessThan, true)]
   [InlineData(ImplementedComparisonOperators.LessThanOrEqual, ImplementedComparisonOperators.LessThanOrEqual, true)]
   [InlineData(ImplementedComparisonOperators.GreaterThan, ImplementedComparisonOperators.LessThan, false)]
   [InlineData(ImplementedComparisonOperators.LessThan, ImplementedComparisonOperators.GreaterThan, false)]
   [InlineData(ImplementedComparisonOperators.None, ImplementedComparisonOperators.GreaterThan, false)]
   [InlineData(ImplementedComparisonOperators.None, ImplementedComparisonOperators.LessThan, false)]
   public void Should_handle_single_flag_checks(
      ImplementedComparisonOperators operators,
      ImplementedComparisonOperators operatorToCheckFor,
      bool expected)
   {
      var result = operators.HasOperator(operatorToCheckFor);

      result.Should().Be(expected);
   }

   [Theory]
   [InlineData(ImplementedComparisonOperators.All, ImplementedComparisonOperators.GreaterThan, true)]
   [InlineData(ImplementedComparisonOperators.All, ImplementedComparisonOperators.GreaterThanOrEqual, true)]
   [InlineData(ImplementedComparisonOperators.All, ImplementedComparisonOperators.LessThan, true)]
   [InlineData(ImplementedComparisonOperators.All, ImplementedComparisonOperators.LessThanOrEqual, true)]
   [InlineData(ImplementedComparisonOperators.All, ImplementedComparisonOperators.All, true)]
   public void Should_return_true_when_All_contains_any_operator(
      ImplementedComparisonOperators operators,
      ImplementedComparisonOperators operatorToCheckFor,
      bool expected)
   {
      var result = operators.HasOperator(operatorToCheckFor);

      result.Should().Be(expected);
   }

   [Theory]
   [InlineData(ImplementedComparisonOperators.GreaterThan | ImplementedComparisonOperators.LessThan, ImplementedComparisonOperators.GreaterThan, true)]
   [InlineData(ImplementedComparisonOperators.GreaterThan | ImplementedComparisonOperators.LessThan, ImplementedComparisonOperators.LessThan, true)]
   [InlineData(ImplementedComparisonOperators.GreaterThan | ImplementedComparisonOperators.LessThan, ImplementedComparisonOperators.GreaterThanOrEqual, false)]
   [InlineData(ImplementedComparisonOperators.GreaterThanOrEqual | ImplementedComparisonOperators.LessThanOrEqual, ImplementedComparisonOperators.GreaterThanOrEqual, true)]
   [InlineData(ImplementedComparisonOperators.GreaterThanOrEqual | ImplementedComparisonOperators.LessThanOrEqual, ImplementedComparisonOperators.LessThanOrEqual, true)]
   [InlineData(ImplementedComparisonOperators.GreaterThanOrEqual | ImplementedComparisonOperators.LessThanOrEqual, ImplementedComparisonOperators.GreaterThan, false)]
   public void Should_handle_multiple_flag_combinations(
      ImplementedComparisonOperators operators,
      ImplementedComparisonOperators operatorToCheckFor,
      bool expected)
   {
      var result = operators.HasOperator(operatorToCheckFor);

      result.Should().Be(expected);
   }

   [Theory]
   [InlineData(ImplementedComparisonOperators.GreaterThan, ImplementedComparisonOperators.All, false)]
   [InlineData(ImplementedComparisonOperators.LessThan, ImplementedComparisonOperators.All, false)]
   [InlineData(ImplementedComparisonOperators.GreaterThan | ImplementedComparisonOperators.LessThan, ImplementedComparisonOperators.All, false)]
   public void Should_return_false_when_checking_for_multiple_flags_but_not_all_exist(
      ImplementedComparisonOperators operators,
      ImplementedComparisonOperators operatorToCheckFor,
      bool expected)
   {
      var result = operators.HasOperator(operatorToCheckFor);

      result.Should().Be(expected);
   }

   [Fact]
   public void Should_return_false_when_operators_is_None_and_checking_for_any_flag()
   {
      var operators = ImplementedComparisonOperators.None;

      operators.HasOperator(ImplementedComparisonOperators.GreaterThan).Should().BeFalse();
      operators.HasOperator(ImplementedComparisonOperators.GreaterThanOrEqual).Should().BeFalse();
      operators.HasOperator(ImplementedComparisonOperators.LessThan).Should().BeFalse();
      operators.HasOperator(ImplementedComparisonOperators.LessThanOrEqual).Should().BeFalse();
      operators.HasOperator(ImplementedComparisonOperators.All).Should().BeFalse();
   }

   [Theory]
   [InlineData(ImplementedComparisonOperators.GreaterThan | ImplementedComparisonOperators.GreaterThanOrEqual, ImplementedComparisonOperators.GreaterThan | ImplementedComparisonOperators.GreaterThanOrEqual, true)]
   [InlineData(ImplementedComparisonOperators.LessThan | ImplementedComparisonOperators.LessThanOrEqual, ImplementedComparisonOperators.LessThan | ImplementedComparisonOperators.LessThanOrEqual, true)]
   [InlineData(ImplementedComparisonOperators.All, ImplementedComparisonOperators.GreaterThan | ImplementedComparisonOperators.LessThan, true)]
   public void Should_return_true_when_checking_for_exact_combination_match(
      ImplementedComparisonOperators operators,
      ImplementedComparisonOperators operatorToCheckFor,
      bool expected)
   {
      var result = operators.HasOperator(operatorToCheckFor);

      result.Should().Be(expected);
   }
}

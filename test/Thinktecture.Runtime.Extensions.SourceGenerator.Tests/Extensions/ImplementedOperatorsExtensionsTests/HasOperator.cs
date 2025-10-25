using Thinktecture.CodeAnalysis;

#nullable enable

namespace Thinktecture.Runtime.Tests.ImplementedOperatorsExtensionsTests;

public class HasOperator
{
   [Fact]
   public void Should_return_false_when_checking_for_None()
   {
      var operators = ImplementedOperators.Default;

      var result = operators.HasOperator(ImplementedOperators.None);

      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_when_both_are_None()
   {
      var operators = ImplementedOperators.None;

      var result = operators.HasOperator(ImplementedOperators.None);

      result.Should().BeFalse();
   }

   [Theory]
   [InlineData(ImplementedOperators.Default, ImplementedOperators.Default, true)]
   [InlineData(ImplementedOperators.Checked, ImplementedOperators.Checked, true)]
   [InlineData(ImplementedOperators.Default, ImplementedOperators.Checked, false)]
   [InlineData(ImplementedOperators.Checked, ImplementedOperators.Default, false)]
   [InlineData(ImplementedOperators.None, ImplementedOperators.Default, false)]
   [InlineData(ImplementedOperators.None, ImplementedOperators.Checked, false)]
   public void Should_handle_single_flag_checks(
      ImplementedOperators operators,
      ImplementedOperators operatorToCheckFor,
      bool expected)
   {
      var result = operators.HasOperator(operatorToCheckFor);

      result.Should().Be(expected);
   }

   [Theory]
   [InlineData(ImplementedOperators.All, ImplementedOperators.Default, true)]
   [InlineData(ImplementedOperators.All, ImplementedOperators.Checked, true)]
   [InlineData(ImplementedOperators.All, ImplementedOperators.All, true)]
   public void Should_return_true_when_checking_for_flag_that_exists_in_combination(
      ImplementedOperators operators,
      ImplementedOperators operatorToCheckFor,
      bool expected)
   {
      var result = operators.HasOperator(operatorToCheckFor);

      result.Should().Be(expected);
   }

   [Theory]
   [InlineData(ImplementedOperators.Default, ImplementedOperators.All, false)]
   [InlineData(ImplementedOperators.Checked, ImplementedOperators.All, false)]
   public void Should_return_false_when_checking_for_multiple_flags_but_only_one_exists(
      ImplementedOperators operators,
      ImplementedOperators operatorToCheckFor,
      bool expected)
   {
      var result = operators.HasOperator(operatorToCheckFor);

      result.Should().Be(expected);
   }

   [Fact]
   public void Should_return_false_when_operators_is_None_and_checking_for_any_flag()
   {
      var operators = ImplementedOperators.None;

      operators.HasOperator(ImplementedOperators.Default).Should().BeFalse();
      operators.HasOperator(ImplementedOperators.Checked).Should().BeFalse();
      operators.HasOperator(ImplementedOperators.All).Should().BeFalse();
   }
}

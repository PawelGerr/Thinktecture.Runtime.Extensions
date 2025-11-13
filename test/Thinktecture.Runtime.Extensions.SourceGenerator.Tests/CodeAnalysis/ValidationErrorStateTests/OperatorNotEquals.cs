using Thinktecture.CodeAnalysis;

#nullable enable

namespace Thinktecture.Runtime.Tests.CodeAnalysis.ValidationErrorStateTests;

public class OperatorNotEquals
{
   [Fact]
   public void Should_return_false_for_equal_instances()
   {
      // Arrange
      var state1 = new ValidationErrorState("global::MyType");
      var state2 = new ValidationErrorState("global::MyType");

      // Act
      var result = state1 != state2;

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_true_for_different_instances()
   {
      // Arrange
      var state1 = new ValidationErrorState("global::MyType");
      var state2 = new ValidationErrorState("global::OtherType");

      // Act
      var result = state1 != state2;

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_false_for_same_instance()
   {
      // Arrange
      var state = new ValidationErrorState("global::MyType");

      // Act
#pragma warning disable CS1718 // Comparison made to same variable
      // ReSharper disable once EqualExpressionComparison
      var result = state != state;
#pragma warning restore CS1718

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_for_default_instances()
   {
      // Arrange
      var state1 = ValidationErrorState.Default;
      var state2 = ValidationErrorState.Default;

      // Act
      var result = state1 != state2;

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_be_opposite_of_equals_operator()
   {
      // Arrange
      var state1 = new ValidationErrorState("global::MyType");
      var state2 = new ValidationErrorState("global::MyType");

      // Act
      var equalsResult = state1 == state2;
      var notEqualsResult = state1 != state2;

      // Assert
      notEqualsResult.Should().Be(!equalsResult);
   }

   [Fact]
   public void Should_be_consistent_with_equals_method()
   {
      // Arrange
      var state1 = new ValidationErrorState("global::MyType");
      var state2 = new ValidationErrorState("global::OtherType");

      // Act
      var operatorResult = state1 != state2;
      var equalsResult = !state1.Equals(state2);

      // Assert
      operatorResult.Should().Be(equalsResult);
   }

   [Fact]
   public void Should_be_case_sensitive()
   {
      // Arrange
      var state1 = new ValidationErrorState("global::MyType");
      var state2 = new ValidationErrorState("global::mytype");

      // Act
      var result = state1 != state2;

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_for_whitespace_differences()
   {
      // Arrange
      var state1 = new ValidationErrorState("global::MyType");
      var state2 = new ValidationErrorState("global::MyType ");

      // Act
      var result = state1 != state2;

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_false_for_empty_strings()
   {
      // Arrange
      var state1 = new ValidationErrorState(string.Empty);
      var state2 = new ValidationErrorState(string.Empty);

      // Act
      var result = state1 != state2;

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_be_reflexive()
   {
      // Arrange
      var state = new ValidationErrorState("global::MyType");

      // Act
#pragma warning disable CS1718 // Comparison made to same variable
      var result = state != state;
#pragma warning restore CS1718

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_be_symmetric()
   {
      // Arrange
      var state1 = new ValidationErrorState("global::MyType");
      var state2 = new ValidationErrorState("global::OtherType");

      // Act
      var result1 = state1 != state2;
      var result2 = state2 != state1;

      // Assert
      result1.Should().Be(result2);
   }

   [Fact]
   public void Should_handle_unicode_characters()
   {
      // Arrange
      var state1 = new ValidationErrorState("global::Type_\u00E9\u4E2D\u6587");
      var state2 = new ValidationErrorState("global::Type_\u00E9\u4E2D\u6587");

      // Act
      var result = state1 != state2;

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_handle_very_long_strings()
   {
      // Arrange
      var longString = "global::" + new string('A', 10000);
      var state1 = new ValidationErrorState(longString);
      var state2 = new ValidationErrorState(longString);

      // Act
      var result = state1 != state2;

      // Assert
      result.Should().BeFalse();
   }

   [Theory]
   [InlineData("global::Thinktecture.ValidationError", "global::Thinktecture.ValidationError", false)]
   [InlineData("global::MyType", "global::MyType", false)]
   [InlineData("global::MyType", "global::OtherType", true)]
   [InlineData("", "", false)]
   [InlineData("A", "B", true)]
   [InlineData("global::Type", "global::type", true)]
   public void Should_handle_various_inequality_scenarios(string type1, string type2, bool expectedNotEqual)
   {
      // Arrange
      var state1 = new ValidationErrorState(type1);
      var state2 = new ValidationErrorState(type2);

      // Act
      var result = state1 != state2;

      // Assert
      result.Should().Be(expectedNotEqual);
   }

   [Fact]
   public void Should_work_in_conditional_expressions()
   {
      // Arrange
      var state1 = new ValidationErrorState("global::MyType");
      var state2 = new ValidationErrorState("global::OtherType");

      // Act
      var message = state1 != state2 ? "Not Equal" : "Equal";

      // Assert
      message.Should().Be("Not Equal");
   }

   [Fact]
   public void Should_work_in_if_statements()
   {
      // Arrange
      var state1 = new ValidationErrorState("global::MyType");
      var state2 = new ValidationErrorState("global::OtherType");
      var inequalityDetected = false;

      // Act
      if (state1 != state2)
      {
         inequalityDetected = true;
      }

      // Assert
      inequalityDetected.Should().BeTrue();
   }

   [Fact]
   public void Should_satisfy_law_of_excluded_middle_with_equals_operator()
   {
      // Arrange
      var state1 = new ValidationErrorState("global::MyType");
      var state2 = new ValidationErrorState("global::OtherType");

      // Act & Assert
      // Either state1 == state2 or state1 != state2, but not both
      var isEqual = state1 == state2;
      var isNotEqual = state1 != state2;

      (isEqual || isNotEqual).Should().BeTrue();
      (isEqual && isNotEqual).Should().BeFalse();
   }
}

using Thinktecture.CodeAnalysis;

#nullable enable

namespace Thinktecture.Runtime.Tests.CodeAnalysis.ValidationErrorStateTests;

public class TypeFullyQualified
{
   [Fact]
   public void Should_return_value_provided_to_constructor()
   {
      // Arrange
      const string expected = "global::MyNamespace.MyValidationError";
      var state = new ValidationErrorState(expected);

      // Act
      var actual = state.TypeFullyQualified;

      // Assert
      actual.Should().Be(expected);
   }

   [Fact]
   public void Should_return_same_reference_for_multiple_calls()
   {
      // Arrange
      const string expected = "global::MyNamespace.MyValidationError";
      var state = new ValidationErrorState(expected);

      // Act
      var first = state.TypeFullyQualified;
      var second = state.TypeFullyQualified;

      // Assert
      first.Should().BeSameAs(second);
   }

   [Fact]
   public void Should_return_empty_string_when_constructed_with_empty_string()
   {
      // Arrange
      var state = new ValidationErrorState(string.Empty);

      // Act
      var actual = state.TypeFullyQualified;

      // Assert
      actual.Should().BeEmpty();
   }

   [Theory]
   [InlineData("global::Thinktecture.ValidationError")]
   [InlineData("System.ComponentModel.DataAnnotations.ValidationResult")]
   [InlineData("MyApp.Errors.CustomError")]
   [InlineData("A")]
   [InlineData("")]
   public void Should_return_exact_input_for_various_types(string input)
   {
      // Arrange
      var state = new ValidationErrorState(input);

      // Act
      var actual = state.TypeFullyQualified;

      // Assert
      actual.Should().Be(input);
   }

   [Fact]
   public void Should_be_case_sensitive()
   {
      // Arrange
      var state1 = new ValidationErrorState("global::MyType");
      var state2 = new ValidationErrorState("global::mytype");

      // Act
      var value1 = state1.TypeFullyQualified;
      var value2 = state2.TypeFullyQualified;

      // Assert
      value1.Should().NotBe(value2);
   }

   [Fact]
   public void Should_preserve_whitespace()
   {
      // Arrange
      const string typeWithWhitespace = "  global::MyType  ";
      var state = new ValidationErrorState(typeWithWhitespace);

      // Act
      var actual = state.TypeFullyQualified;

      // Assert
      actual.Should().Be(typeWithWhitespace);
   }
}

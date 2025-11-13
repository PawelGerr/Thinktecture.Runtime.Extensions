using Thinktecture.CodeAnalysis;

#nullable enable

namespace Thinktecture.Runtime.Tests.CodeAnalysis.ValidationErrorStateTests;

public class ToString
{
   [Fact]
   public void Should_return_type_fully_qualified()
   {
      // Arrange
      const string typeFullyQualified = "global::MyNamespace.MyValidationError";
      var state = new ValidationErrorState(typeFullyQualified);

      // Act
      var result = state.ToString();

      // Assert
      result.Should().Be(typeFullyQualified);
   }

   [Fact]
   public void Should_return_empty_string_when_type_is_empty()
   {
      // Arrange
      var state = new ValidationErrorState(string.Empty);

      // Act
      var result = state.ToString();

      // Assert
      result.Should().BeEmpty();
   }

   [Fact]
   public void Should_return_same_reference_as_type_fully_qualified()
   {
      // Arrange
      const string typeFullyQualified = "global::MyNamespace.MyValidationError";
      var state = new ValidationErrorState(typeFullyQualified);

      // Act
      var toString = state.ToString();
      var property = state.TypeFullyQualified;

      // Assert
      toString.Should().BeSameAs(property);
   }

   [Fact]
   public void Should_return_default_value_for_default_instance()
   {
      // Arrange
      var state = ValidationErrorState.Default;

      // Act
      var result = state.ToString();

      // Assert
      result.Should().Be("global::Thinktecture.ValidationError");
   }

   [Fact]
   public void Should_preserve_whitespace()
   {
      // Arrange
      const string typeWithWhitespace = "  global::MyType  ";
      var state = new ValidationErrorState(typeWithWhitespace);

      // Act
      var result = state.ToString();

      // Assert
      result.Should().Be(typeWithWhitespace);
   }

   [Fact]
   public void Should_preserve_special_characters()
   {
      // Arrange
      const string typeWithSpecialChars = "global::Namespace<T>.Type<U, V>.Nested`2";
      var state = new ValidationErrorState(typeWithSpecialChars);

      // Act
      var result = state.ToString();

      // Assert
      result.Should().Be(typeWithSpecialChars);
   }

   [Fact]
   public void Should_preserve_unicode_characters()
   {
      // Arrange
      const string typeWithUnicode = "global::Type_\u00E9\u4E2D\u6587";
      var state = new ValidationErrorState(typeWithUnicode);

      // Act
      var result = state.ToString();

      // Assert
      result.Should().Be(typeWithUnicode);
   }

   [Fact]
   public void Should_preserve_newlines_and_tabs()
   {
      // Arrange
      const string typeWithControlChars = "global::Type\nWith\tSpecial";
      var state = new ValidationErrorState(typeWithControlChars);

      // Act
      var result = state.ToString();

      // Assert
      result.Should().Be(typeWithControlChars);
   }

   [Fact]
   public void Should_handle_very_long_strings()
   {
      // Arrange
      var longString = "global::" + new string('A', 10000);
      var state = new ValidationErrorState(longString);

      // Act
      var result = state.ToString();

      // Assert
      result.Should().Be(longString);
   }

   [Fact]
   public void Should_return_consistent_value_for_multiple_calls()
   {
      // Arrange
      var state = new ValidationErrorState("global::MyType");

      // Act
      var result1 = state.ToString();
      var result2 = state.ToString();
      var result3 = state.ToString();

      // Assert
      result1.Should().Be(result2);
      result2.Should().Be(result3);
      result1.Should().BeSameAs(result2);
      result2.Should().BeSameAs(result3);
   }

   [Theory]
   [InlineData("global::Thinktecture.ValidationError")]
   [InlineData("System.ComponentModel.DataAnnotations.ValidationResult")]
   [InlineData("MyApp.Errors.CustomError")]
   [InlineData("A")]
   [InlineData("")]
   [InlineData(" ")]
   public void Should_return_exact_input_for_various_types(string input)
   {
      // Arrange
      var state = new ValidationErrorState(input);

      // Act
      var result = state.ToString();

      // Assert
      result.Should().Be(input);
   }

   [Fact]
   public void Should_be_usable_in_string_interpolation()
   {
      // Arrange
      var state = new ValidationErrorState("global::MyType");

      // Act
      var interpolated = $"Type: {state}";

      // Assert
      interpolated.Should().Be("Type: global::MyType");
   }

   [Fact]
   public void Should_be_usable_in_string_concatenation()
   {
      // Arrange
      var state = new ValidationErrorState("global::MyType");

      // Act
      var concatenated = "Type: " + state;

      // Assert
      concatenated.Should().Be("Type: global::MyType");
   }
}

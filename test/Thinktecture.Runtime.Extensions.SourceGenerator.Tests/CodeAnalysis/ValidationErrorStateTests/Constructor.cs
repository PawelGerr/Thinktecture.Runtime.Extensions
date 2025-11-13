using Thinktecture.CodeAnalysis;

#nullable enable

namespace Thinktecture.Runtime.Tests.CodeAnalysis.ValidationErrorStateTests;

public class Constructor
{
   [Fact]
   public void Should_initialize_type_fully_qualified_with_provided_value()
   {
      // Arrange
      const string typeFullyQualified = "global::MyNamespace.MyValidationError";

      // Act
      var state = new ValidationErrorState(typeFullyQualified);

      // Assert
      state.TypeFullyQualified.Should().Be(typeFullyQualified);
   }

   [Fact]
   public void Should_handle_empty_string()
   {
      // Arrange
      const string typeFullyQualified = "";

      // Act
      var state = new ValidationErrorState(typeFullyQualified);

      // Assert
      state.TypeFullyQualified.Should().Be(string.Empty);
   }

   [Fact]
   public void Should_handle_whitespace_string()
   {
      // Arrange
      const string typeFullyQualified = "   ";

      // Act
      var state = new ValidationErrorState(typeFullyQualified);

      // Assert
      state.TypeFullyQualified.Should().Be("   ");
   }

   [Fact]
   public void Should_handle_string_with_newlines()
   {
      // Arrange
      const string typeFullyQualified = "global::Namespace.Type\nWithNewline";

      // Act
      var state = new ValidationErrorState(typeFullyQualified);

      // Assert
      state.TypeFullyQualified.Should().Be(typeFullyQualified);
   }

   [Fact]
   public void Should_handle_string_with_tabs()
   {
      // Arrange
      const string typeFullyQualified = "global::Namespace.Type\tWithTab";

      // Act
      var state = new ValidationErrorState(typeFullyQualified);

      // Assert
      state.TypeFullyQualified.Should().Be(typeFullyQualified);
   }

   [Fact]
   public void Should_handle_unicode_characters()
   {
      // Arrange
      const string typeFullyQualified = "global::Namespace.Type_\u00E9\u4E2D\u6587";

      // Act
      var state = new ValidationErrorState(typeFullyQualified);

      // Assert
      state.TypeFullyQualified.Should().Be(typeFullyQualified);
   }

   [Fact]
   public void Should_handle_very_long_string()
   {
      // Arrange
      var typeFullyQualified = "global::" + new string('A', 10000);

      // Act
      var state = new ValidationErrorState(typeFullyQualified);

      // Assert
      state.TypeFullyQualified.Should().Be(typeFullyQualified);
   }

   [Fact]
   public void Should_handle_string_with_special_characters()
   {
      // Arrange
      const string typeFullyQualified = "global::Namespace<T>.Type<U, V>.Nested`2";

      // Act
      var state = new ValidationErrorState(typeFullyQualified);

      // Assert
      state.TypeFullyQualified.Should().Be(typeFullyQualified);
   }

   [Fact]
   public void Should_handle_string_with_dots_and_colons()
   {
      // Arrange
      const string typeFullyQualified = "global::System.Collections.Generic.List`1";

      // Act
      var state = new ValidationErrorState(typeFullyQualified);

      // Assert
      state.TypeFullyQualified.Should().Be(typeFullyQualified);
   }

   [Theory]
   [InlineData("global::Thinktecture.ValidationError")]
   [InlineData("System.String")]
   [InlineData("MyType")]
   [InlineData("")]
   [InlineData(" ")]
   public void Should_preserve_exact_input_string(string input)
   {
      // Arrange & Act
      var state = new ValidationErrorState(input);

      // Assert
      state.TypeFullyQualified.Should().Be(input);
   }
}

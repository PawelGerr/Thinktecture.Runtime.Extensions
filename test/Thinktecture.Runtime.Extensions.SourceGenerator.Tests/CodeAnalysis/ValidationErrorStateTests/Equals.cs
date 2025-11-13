using Thinktecture.CodeAnalysis;

#nullable enable

namespace Thinktecture.Runtime.Tests.CodeAnalysis.ValidationErrorStateTests;

public class Equals
{
   [Fact]
   public void Should_return_true_for_same_instance()
   {
      // Arrange
      var state = new ValidationErrorState("global::MyType");

      // Act
      var result = state.Equals(state);

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_for_equal_instances_with_same_type()
   {
      // Arrange
      var state1 = new ValidationErrorState("global::MyType");
      var state2 = new ValidationErrorState("global::MyType");

      // Act
      var result = state1.Equals(state2);

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_false_for_instances_with_different_types()
   {
      // Arrange
      var state1 = new ValidationErrorState("global::MyType");
      var state2 = new ValidationErrorState("global::OtherType");

      // Act
      var result = state1.Equals(state2);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_be_case_sensitive()
   {
      // Arrange
      var state1 = new ValidationErrorState("global::MyType");
      var state2 = new ValidationErrorState("global::mytype");

      // Act
      var result = state1.Equals(state2);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_for_different_whitespace()
   {
      // Arrange
      var state1 = new ValidationErrorState("global::MyType");
      var state2 = new ValidationErrorState("global::MyType ");

      // Act
      var result = state1.Equals(state2);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_true_for_empty_strings()
   {
      // Arrange
      var state1 = new ValidationErrorState(string.Empty);
      var state2 = new ValidationErrorState(string.Empty);

      // Act
      var result = state1.Equals(state2);

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_for_default_instances()
   {
      // Arrange
      var state1 = ValidationErrorState.Default;
      var state2 = ValidationErrorState.Default;

      // Act
      var result = state1.Equals(state2);

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_be_reflexive()
   {
      // Arrange
      var state = new ValidationErrorState("global::MyType");

      // Act & Assert
      state.Equals(state).Should().BeTrue();
   }

   [Fact]
   public void Should_be_symmetric()
   {
      // Arrange
      var state1 = new ValidationErrorState("global::MyType");
      var state2 = new ValidationErrorState("global::MyType");

      // Act & Assert
      state1.Equals(state2).Should().Be(state2.Equals(state1));
   }

   [Fact]
   public void Should_be_transitive()
   {
      // Arrange
      var state1 = new ValidationErrorState("global::MyType");
      var state2 = new ValidationErrorState("global::MyType");
      var state3 = new ValidationErrorState("global::MyType");

      // Act & Assert
      if (state1.Equals(state2) && state2.Equals(state3))
      {
         state1.Equals(state3).Should().BeTrue();
      }
   }

   [Fact]
   public void Should_handle_unicode_characters()
   {
      // Arrange
      var state1 = new ValidationErrorState("global::Type_\u00E9\u4E2D\u6587");
      var state2 = new ValidationErrorState("global::Type_\u00E9\u4E2D\u6587");

      // Act
      var result = state1.Equals(state2);

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_very_long_strings()
   {
      // Arrange
      var longString = "global::" + new string('A', 10000);
      var state1 = new ValidationErrorState(longString);
      var state2 = new ValidationErrorState(longString);

      // Act
      var result = state1.Equals(state2);

      // Assert
      result.Should().BeTrue();
   }

   // Equals(object) overload tests

   [Fact]
   public void Should_return_true_via_object_overload_for_equal_instances()
   {
      // Arrange
      var state1 = new ValidationErrorState("global::MyType");
      object state2 = new ValidationErrorState("global::MyType");

      // Act
      var result = state1.Equals(state2);

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_false_via_object_overload_for_null()
   {
      // Arrange
      var state = new ValidationErrorState("global::MyType");
      object? nullObject = null;

      // Act
      var result = state.Equals(nullObject);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_via_object_overload_for_different_type()
   {
      // Arrange
      var state = new ValidationErrorState("global::MyType");
      var differentType = new object();

      // Act
      var result = state.Equals(differentType);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_via_object_overload_for_string_with_same_value()
   {
      // Arrange
      var state = new ValidationErrorState("global::MyType");
      object stringObject = "global::MyType";

      // Act
      var result = state.Equals(stringObject);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_via_object_overload_for_different_instances()
   {
      // Arrange
      var state1 = new ValidationErrorState("global::MyType");
      object state2 = new ValidationErrorState("global::OtherType");

      // Act
      var result = state1.Equals(state2);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_true_via_object_overload_for_boxed_same_instance()
   {
      // Arrange
      var state = new ValidationErrorState("global::MyType");
      object boxed = state;

      // Act
      var result = state.Equals(boxed);

      // Assert
      result.Should().BeTrue();
   }

   [Theory]
   [InlineData("global::Thinktecture.ValidationError", "global::Thinktecture.ValidationError", true)]
   [InlineData("global::MyType", "global::MyType", true)]
   [InlineData("global::MyType", "global::OtherType", false)]
   [InlineData("", "", true)]
   [InlineData("A", "B", false)]
   [InlineData("global::Type", "global::type", false)]
   public void Should_handle_various_equality_scenarios(string type1, string type2, bool expectedEqual)
   {
      // Arrange
      var state1 = new ValidationErrorState(type1);
      var state2 = new ValidationErrorState(type2);

      // Act
      var result = state1.Equals(state2);

      // Assert
      result.Should().Be(expectedEqual);
   }
}

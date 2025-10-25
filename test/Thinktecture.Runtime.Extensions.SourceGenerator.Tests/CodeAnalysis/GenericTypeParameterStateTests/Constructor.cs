using System.Collections.Immutable;
using Thinktecture.CodeAnalysis;

namespace Thinktecture.Runtime.Tests.CodeAnalysis.GenericTypeParameterStateTests;

public class Constructor
{
   [Fact]
   public void Should_initialize_name_property()
   {
      // Arrange & Act
      var state = new GenericTypeParameterState("T", ImmutableArray<string>.Empty);

      // Assert
      state.Name.Should().Be("T");
   }

   [Fact]
   public void Should_initialize_constraints_property()
   {
      // Arrange
      var constraints = ImmutableArray.Create("class", "IDisposable");

      // Act
      var state = new GenericTypeParameterState("T", constraints);

      // Assert
      state.Constraints.Should().Equal(constraints);
   }

   [Fact]
   public void Should_handle_empty_name()
   {
      // Arrange & Act
      var state = new GenericTypeParameterState("", ImmutableArray<string>.Empty);

      // Assert
      state.Name.Should().Be("");
   }

   [Fact]
   public void Should_handle_empty_constraints()
   {
      // Arrange & Act
      var state = new GenericTypeParameterState("T", ImmutableArray<string>.Empty);

      // Assert
      state.Constraints.Should().BeEmpty();
   }

   [Fact]
   public void Should_handle_single_constraint()
   {
      // Arrange
      var constraints = ImmutableArray.Create("class");

      // Act
      var state = new GenericTypeParameterState("T", constraints);

      // Assert
      state.Constraints.Should().HaveCount(1);
      state.Constraints[0].Should().Be("class");
   }

   [Fact]
   public void Should_handle_multiple_constraints()
   {
      // Arrange
      var constraints = ImmutableArray.Create("class", "IDisposable", "new()");

      // Act
      var state = new GenericTypeParameterState("T", constraints);

      // Assert
      state.Constraints.Should().HaveCount(3);
      state.Constraints.Should().Equal("class", "IDisposable", "new()");
   }

   [Fact]
   public void Should_handle_name_with_special_characters()
   {
      // Arrange & Act
      var state = new GenericTypeParameterState("T_Value123", ImmutableArray<string>.Empty);

      // Assert
      state.Name.Should().Be("T_Value123");
   }

   [Fact]
   public void Should_preserve_constraints_order()
   {
      // Arrange
      var constraints = ImmutableArray.Create("IDisposable", "class", "new()");

      // Act
      var state = new GenericTypeParameterState("T", constraints);

      // Assert
      state.Constraints.Should().Equal("IDisposable", "class", "new()");
   }
}

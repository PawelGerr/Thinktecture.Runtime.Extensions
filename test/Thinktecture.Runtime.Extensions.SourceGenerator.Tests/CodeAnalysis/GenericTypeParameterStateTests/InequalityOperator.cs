using System.Collections.Immutable;
using Thinktecture.CodeAnalysis;

#nullable enable

namespace Thinktecture.Runtime.Tests.CodeAnalysis.GenericTypeParameterStateTests;

public class InequalityOperator
{
   [Fact]
   public void Should_return_false_for_same_instance()
   {
      // Arrange
      var state = new GenericTypeParameterState("T", ImmutableArray<string>.Empty);

      // Act
#pragma warning disable CS1718 // Comparison made to same variable
      // ReSharper disable once EqualExpressionComparison
      var result = state != state;
#pragma warning restore CS1718 // Comparison made to same variable

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_for_equal_instances_with_empty_constraints()
   {
      // Arrange
      var state1 = new GenericTypeParameterState("T", ImmutableArray<string>.Empty);
      var state2 = new GenericTypeParameterState("T", ImmutableArray<string>.Empty);

      // Act
      var result = state1 != state2;

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_for_equal_instances_with_same_constraints()
   {
      // Arrange
      var constraints = ImmutableArray.Create("class", "IDisposable");
      var state1 = new GenericTypeParameterState("T", constraints);
      var state2 = new GenericTypeParameterState("T", constraints);

      // Act
      var result = state1 != state2;

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_true_for_different_names()
   {
      // Arrange
      var state1 = new GenericTypeParameterState("T", ImmutableArray<string>.Empty);
      var state2 = new GenericTypeParameterState("TValue", ImmutableArray<string>.Empty);

      // Act
      var result = state1 != state2;

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_for_different_constraints()
   {
      // Arrange
      var constraints1 = ImmutableArray.Create("class");
      var constraints2 = ImmutableArray.Create("struct");
      var state1 = new GenericTypeParameterState("T", constraints1);
      var state2 = new GenericTypeParameterState("T", constraints2);

      // Act
      var result = state1 != state2;

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_for_different_constraint_order()
   {
      // Arrange
      var constraints1 = ImmutableArray.Create("class", "IDisposable");
      var constraints2 = ImmutableArray.Create("IDisposable", "class");
      var state1 = new GenericTypeParameterState("T", constraints1);
      var state2 = new GenericTypeParameterState("T", constraints2);

      // Act
      var result = state1 != state2;

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_null_on_left_side()
   {
      // Arrange
      GenericTypeParameterState? left = null;
      var right = new GenericTypeParameterState("T", ImmutableArray<string>.Empty);

      // Act
      var result = left != right;

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_null_on_right_side()
   {
      // Arrange
      var left = new GenericTypeParameterState("T", ImmutableArray<string>.Empty);
      GenericTypeParameterState? right = null;

      // Act
      var result = left != right;

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_both_sides_null()
   {
      // Arrange
      GenericTypeParameterState? left = null;
      GenericTypeParameterState? right = null;

      // Act
      var result = left != right;

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_be_inverse_of_equality_operator()
   {
      // Arrange
      var constraints1 = ImmutableArray.Create("class", "IDisposable");
      var constraints2 = ImmutableArray.Create("class", "IDisposable");
      var state1 = new GenericTypeParameterState("T", constraints1);
      var state2 = new GenericTypeParameterState("T", constraints2);

      // Act & Assert
      (state1 != state2).Should().Be(!(state1 == state2));
   }

   [Fact]
   public void Should_be_symmetric()
   {
      // Arrange
      var constraints1 = ImmutableArray.Create("class");
      var constraints2 = ImmutableArray.Create("struct");
      var state1 = new GenericTypeParameterState("T", constraints1);
      var state2 = new GenericTypeParameterState("T", constraints2);

      // Act & Assert
      (state1 != state2).Should().Be(state2 != state1);
   }
}

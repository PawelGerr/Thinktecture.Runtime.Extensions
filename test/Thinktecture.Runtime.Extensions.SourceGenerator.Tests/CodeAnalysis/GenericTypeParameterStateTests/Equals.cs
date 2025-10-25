using System.Collections.Immutable;
using Thinktecture.CodeAnalysis;

#nullable enable

namespace Thinktecture.Runtime.Tests.CodeAnalysis.GenericTypeParameterStateTests;

public class Equals
{
   [Fact]
   public void Should_return_true_for_same_instance()
   {
      // Arrange
      var state = new GenericTypeParameterState("T", ImmutableArray<string>.Empty);

      // Act
      var result = state.Equals(state);

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_for_reference_equal_instances()
   {
      // Arrange
      var state = new GenericTypeParameterState("T", ImmutableArray<string>.Empty);
      GenericTypeParameterState sameReference = state;

      // Act
      var result = state.Equals(sameReference);

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_for_equal_instances_with_empty_constraints()
   {
      // Arrange
      var state1 = new GenericTypeParameterState("T", ImmutableArray<string>.Empty);
      var state2 = new GenericTypeParameterState("T", ImmutableArray<string>.Empty);

      // Act
      var result = state1.Equals(state2);

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_for_equal_instances_with_same_constraints()
   {
      // Arrange
      var constraints = ImmutableArray.Create("class", "IDisposable");
      var state1 = new GenericTypeParameterState("T", constraints);
      var state2 = new GenericTypeParameterState("T", constraints);

      // Act
      var result = state1.Equals(state2);

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_for_equal_instances_with_equal_constraints()
   {
      // Arrange
      var constraints1 = ImmutableArray.Create("class", "IDisposable");
      var constraints2 = ImmutableArray.Create("class", "IDisposable");
      var state1 = new GenericTypeParameterState("T", constraints1);
      var state2 = new GenericTypeParameterState("T", constraints2);

      // Act
      var result = state1.Equals(state2);

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_false_for_different_names()
   {
      // Arrange
      var state1 = new GenericTypeParameterState("T", ImmutableArray<string>.Empty);
      var state2 = new GenericTypeParameterState("TValue", ImmutableArray<string>.Empty);

      // Act
      var result = state1.Equals(state2);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_for_different_constraints()
   {
      // Arrange
      var constraints1 = ImmutableArray.Create("class");
      var constraints2 = ImmutableArray.Create("struct");
      var state1 = new GenericTypeParameterState("T", constraints1);
      var state2 = new GenericTypeParameterState("T", constraints2);

      // Act
      var result = state1.Equals(state2);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_for_different_number_of_constraints()
   {
      // Arrange
      var constraints1 = ImmutableArray.Create("class");
      var constraints2 = ImmutableArray.Create("class", "IDisposable");
      var state1 = new GenericTypeParameterState("T", constraints1);
      var state2 = new GenericTypeParameterState("T", constraints2);

      // Act
      var result = state1.Equals(state2);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_for_different_constraint_order()
   {
      // Arrange
      var constraints1 = ImmutableArray.Create("class", "IDisposable");
      var constraints2 = ImmutableArray.Create("IDisposable", "class");
      var state1 = new GenericTypeParameterState("T", constraints1);
      var state2 = new GenericTypeParameterState("T", constraints2);

      // Act
      var result = state1.Equals(state2);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_for_null_typed_parameter()
   {
      // Arrange
      var state = new GenericTypeParameterState("T", ImmutableArray<string>.Empty);

      // Act
      var result = state.Equals(null);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_for_different_type_object()
   {
      // Arrange
      var state = new GenericTypeParameterState("T", ImmutableArray<string>.Empty);
      var differentType = new object();

      // Act
      var result = state.Equals(differentType);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_true_for_equal_instances_via_object_overload()
   {
      // Arrange
      var constraints = ImmutableArray.Create("class", "IDisposable");
      var state1 = new GenericTypeParameterState("T", constraints);
      object state2 = new GenericTypeParameterState("T", constraints);

      // Act
      var result = state1.Equals(state2);

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_empty_name_equality()
   {
      // Arrange
      var state1 = new GenericTypeParameterState("", ImmutableArray<string>.Empty);
      var state2 = new GenericTypeParameterState("", ImmutableArray<string>.Empty);

      // Act
      var result = state1.Equals(state2);

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_be_case_sensitive_for_name_comparison()
   {
      // Arrange
      var state1 = new GenericTypeParameterState("T", ImmutableArray<string>.Empty);
      var state2 = new GenericTypeParameterState("t", ImmutableArray<string>.Empty);

      // Act
      var result = state1.Equals(state2);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_be_case_sensitive_for_constraint_comparison()
   {
      // Arrange
      var constraints1 = ImmutableArray.Create("Class");
      var constraints2 = ImmutableArray.Create("class");
      var state1 = new GenericTypeParameterState("T", constraints1);
      var state2 = new GenericTypeParameterState("T", constraints2);

      // Act
      var result = state1.Equals(state2);

      // Assert
      result.Should().BeFalse();
   }
}

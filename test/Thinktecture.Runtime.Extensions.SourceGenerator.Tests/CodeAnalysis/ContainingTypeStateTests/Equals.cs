using System.Collections.Immutable;
using Thinktecture.CodeAnalysis;

#nullable enable

namespace Thinktecture.Runtime.Tests.CodeAnalysis.ContainingTypeStateTests;

public class Equals
{
   [Fact]
   public void Should_return_true_for_same_instance()
   {
      // Arrange
      var state = new ContainingTypeState("MyClass", true, false, ImmutableArray<GenericTypeParameterState>.Empty);

      // Act
      var result = state.Equals(state);

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_for_reference_equal_instances()
   {
      // Arrange
      var state = new ContainingTypeState("MyClass", true, false, ImmutableArray<GenericTypeParameterState>.Empty);
      ContainingTypeState sameReference = state;

      // Act
      var result = state.Equals(sameReference);

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_for_equal_instances_with_empty_generic_parameters()
   {
      // Arrange
      var state1 = new ContainingTypeState("MyClass", true, false, ImmutableArray<GenericTypeParameterState>.Empty);
      var state2 = new ContainingTypeState("MyClass", true, false, ImmutableArray<GenericTypeParameterState>.Empty);

      // Act
      var result = state1.Equals(state2);

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_for_equal_instances_with_same_generic_parameters()
   {
      // Arrange
      var genericParams = ImmutableArray.Create(
         new GenericTypeParameterState("T", ImmutableArray<string>.Empty));
      var state1 = new ContainingTypeState("MyClass", true, false, genericParams);
      var state2 = new ContainingTypeState("MyClass", true, false, genericParams);

      // Act
      var result = state1.Equals(state2);

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_for_equal_instances_with_equal_generic_parameters()
   {
      // Arrange
      var genericParams1 = ImmutableArray.Create(
         new GenericTypeParameterState("T", ImmutableArray<string>.Empty));
      var genericParams2 = ImmutableArray.Create(
         new GenericTypeParameterState("T", ImmutableArray<string>.Empty));
      var state1 = new ContainingTypeState("MyClass", true, false, genericParams1);
      var state2 = new ContainingTypeState("MyClass", true, false, genericParams2);

      // Act
      var result = state1.Equals(state2);

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_for_equal_value_type_instances()
   {
      // Arrange
      var state1 = new ContainingTypeState("MyStruct", false, false, ImmutableArray<GenericTypeParameterState>.Empty);
      var state2 = new ContainingTypeState("MyStruct", false, false, ImmutableArray<GenericTypeParameterState>.Empty);

      // Act
      var result = state1.Equals(state2);

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_for_equal_record_instances()
   {
      // Arrange
      var state1 = new ContainingTypeState("MyRecord", true, true, ImmutableArray<GenericTypeParameterState>.Empty);
      var state2 = new ContainingTypeState("MyRecord", true, true, ImmutableArray<GenericTypeParameterState>.Empty);

      // Act
      var result = state1.Equals(state2);

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_false_for_different_names()
   {
      // Arrange
      var state1 = new ContainingTypeState("MyClass", true, false, ImmutableArray<GenericTypeParameterState>.Empty);
      var state2 = new ContainingTypeState("OtherClass", true, false, ImmutableArray<GenericTypeParameterState>.Empty);

      // Act
      var result = state1.Equals(state2);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_for_different_isReferenceType()
   {
      // Arrange
      var state1 = new ContainingTypeState("MyType", true, false, ImmutableArray<GenericTypeParameterState>.Empty);
      var state2 = new ContainingTypeState("MyType", false, false, ImmutableArray<GenericTypeParameterState>.Empty);

      // Act
      var result = state1.Equals(state2);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_for_different_isRecord()
   {
      // Arrange
      var state1 = new ContainingTypeState("MyType", true, false, ImmutableArray<GenericTypeParameterState>.Empty);
      var state2 = new ContainingTypeState("MyType", true, true, ImmutableArray<GenericTypeParameterState>.Empty);

      // Act
      var result = state1.Equals(state2);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_for_different_generic_parameters()
   {
      // Arrange
      var genericParams1 = ImmutableArray.Create(
         new GenericTypeParameterState("T", ImmutableArray<string>.Empty));
      var genericParams2 = ImmutableArray.Create(
         new GenericTypeParameterState("TKey", ImmutableArray<string>.Empty));
      var state1 = new ContainingTypeState("MyClass", true, false, genericParams1);
      var state2 = new ContainingTypeState("MyClass", true, false, genericParams2);

      // Act
      var result = state1.Equals(state2);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_for_different_number_of_generic_parameters()
   {
      // Arrange
      var genericParams1 = ImmutableArray.Create(
         new GenericTypeParameterState("T", ImmutableArray<string>.Empty));
      var genericParams2 = ImmutableArray.Create(
         new GenericTypeParameterState("T", ImmutableArray<string>.Empty),
         new GenericTypeParameterState("TKey", ImmutableArray<string>.Empty));
      var state1 = new ContainingTypeState("MyClass", true, false, genericParams1);
      var state2 = new ContainingTypeState("MyClass", true, false, genericParams2);

      // Act
      var result = state1.Equals(state2);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_for_different_generic_parameter_order()
   {
      // Arrange
      var genericParams1 = ImmutableArray.Create(
         new GenericTypeParameterState("T", ImmutableArray<string>.Empty),
         new GenericTypeParameterState("TKey", ImmutableArray<string>.Empty));
      var genericParams2 = ImmutableArray.Create(
         new GenericTypeParameterState("TKey", ImmutableArray<string>.Empty),
         new GenericTypeParameterState("T", ImmutableArray<string>.Empty));
      var state1 = new ContainingTypeState("MyClass", true, false, genericParams1);
      var state2 = new ContainingTypeState("MyClass", true, false, genericParams2);

      // Act
      var result = state1.Equals(state2);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_for_empty_vs_non_empty_generic_parameters()
   {
      // Arrange
      var genericParams = ImmutableArray.Create(
         new GenericTypeParameterState("T", ImmutableArray<string>.Empty));
      var state1 = new ContainingTypeState("MyClass", true, false, ImmutableArray<GenericTypeParameterState>.Empty);
      var state2 = new ContainingTypeState("MyClass", true, false, genericParams);

      // Act
      var result = state1.Equals(state2);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_for_null_typed_parameter()
   {
      // Arrange
      var state = new ContainingTypeState("MyClass", true, false, ImmutableArray<GenericTypeParameterState>.Empty);

      // Act
      var result = state.Equals(null);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_for_different_type_object()
   {
      // Arrange
      var state = new ContainingTypeState("MyClass", true, false, ImmutableArray<GenericTypeParameterState>.Empty);
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
      var genericParams = ImmutableArray.Create(
         new GenericTypeParameterState("T", ImmutableArray<string>.Empty));
      var state1 = new ContainingTypeState("MyClass", true, false, genericParams);
      object state2 = new ContainingTypeState("MyClass", true, false, genericParams);

      // Act
      var result = state1.Equals(state2);

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_empty_name_equality()
   {
      // Arrange
      var state1 = new ContainingTypeState("", true, false, ImmutableArray<GenericTypeParameterState>.Empty);
      var state2 = new ContainingTypeState("", true, false, ImmutableArray<GenericTypeParameterState>.Empty);

      // Act
      var result = state1.Equals(state2);

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_be_case_sensitive_for_name_comparison()
   {
      // Arrange
      var state1 = new ContainingTypeState("MyClass", true, false, ImmutableArray<GenericTypeParameterState>.Empty);
      var state2 = new ContainingTypeState("myclass", true, false, ImmutableArray<GenericTypeParameterState>.Empty);

      // Act
      var result = state1.Equals(state2);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_when_all_boolean_flags_differ()
   {
      // Arrange
      var state1 = new ContainingTypeState("MyType", true, true, ImmutableArray<GenericTypeParameterState>.Empty);
      var state2 = new ContainingTypeState("MyType", false, false, ImmutableArray<GenericTypeParameterState>.Empty);

      // Act
      var result = state1.Equals(state2);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_when_only_one_property_differs()
   {
      // Arrange
      var genericParams = ImmutableArray.Create(
         new GenericTypeParameterState("T", ImmutableArray<string>.Empty));
      var state1 = new ContainingTypeState("MyClass", true, false, genericParams);
      var state2 = new ContainingTypeState("MyClass", true, true, genericParams); // Only IsRecord differs

      // Act
      var result = state1.Equals(state2);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_true_for_complex_equal_instances()
   {
      // Arrange
      var genericParams1 = ImmutableArray.Create(
         new GenericTypeParameterState("T", ["class"]),
         new GenericTypeParameterState("TKey", ["struct"]));
      var genericParams2 = ImmutableArray.Create(
         new GenericTypeParameterState("T", ["class"]),
         new GenericTypeParameterState("TKey", ["struct"]));
      var state1 = new ContainingTypeState("MyClass", true, true, genericParams1);
      var state2 = new ContainingTypeState("MyClass", true, true, genericParams2);

      // Act
      var result = state1.Equals(state2);

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_false_for_different_generic_parameter_constraints()
   {
      // Arrange
      var genericParams1 = ImmutableArray.Create(
         new GenericTypeParameterState("T", ["class"]));
      var genericParams2 = ImmutableArray.Create(
         new GenericTypeParameterState("T", ["struct"]));
      var state1 = new ContainingTypeState("MyClass", true, false, genericParams1);
      var state2 = new ContainingTypeState("MyClass", true, false, genericParams2);

      // Act
      var result = state1.Equals(state2);

      // Assert
      result.Should().BeFalse();
   }
}

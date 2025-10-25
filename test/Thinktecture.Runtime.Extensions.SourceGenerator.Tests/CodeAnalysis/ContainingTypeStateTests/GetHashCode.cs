using System.Collections.Immutable;
using Thinktecture.CodeAnalysis;

namespace Thinktecture.Runtime.Tests.CodeAnalysis.ContainingTypeStateTests;

public class GetHashCode
{
   [Fact]
   public void Should_return_same_hash_code_for_same_instance()
   {
      // Arrange
      var state = new ContainingTypeState("MyClass", true, false, ImmutableArray<GenericTypeParameterState>.Empty);

      // Act
      var hashCode1 = state.GetHashCode();
      var hashCode2 = state.GetHashCode();

      // Assert
      hashCode1.Should().Be(hashCode2);
   }

   [Fact]
   public void Should_return_same_hash_code_for_equal_instances()
   {
      // Arrange
      var genericParams = ImmutableArray.Create(
         new GenericTypeParameterState("T", ImmutableArray<string>.Empty));
      var state1 = new ContainingTypeState("MyClass", true, false, genericParams);
      var state2 = new ContainingTypeState("MyClass", true, false, genericParams);

      // Act
      var hashCode1 = state1.GetHashCode();
      var hashCode2 = state2.GetHashCode();

      // Assert
      hashCode1.Should().Be(hashCode2);
   }

   [Fact]
   public void Should_return_same_hash_code_for_instances_with_equal_generic_parameters()
   {
      // Arrange
      var genericParams1 = ImmutableArray.Create(
         new GenericTypeParameterState("T", ImmutableArray<string>.Empty));
      var genericParams2 = ImmutableArray.Create(
         new GenericTypeParameterState("T", ImmutableArray<string>.Empty));
      var state1 = new ContainingTypeState("MyClass", true, false, genericParams1);
      var state2 = new ContainingTypeState("MyClass", true, false, genericParams2);

      // Act
      var hashCode1 = state1.GetHashCode();
      var hashCode2 = state2.GetHashCode();

      // Assert
      hashCode1.Should().Be(hashCode2);
   }

   [Fact]
   public void Should_return_different_hash_code_for_different_names()
   {
      // Arrange
      var state1 = new ContainingTypeState("MyClass", true, false, ImmutableArray<GenericTypeParameterState>.Empty);
      var state2 = new ContainingTypeState("OtherClass", true, false, ImmutableArray<GenericTypeParameterState>.Empty);

      // Act
      var hashCode1 = state1.GetHashCode();
      var hashCode2 = state2.GetHashCode();

      // Assert
      hashCode1.Should().NotBe(hashCode2);
   }

   [Fact]
   public void Should_return_different_hash_code_for_different_isReferenceType()
   {
      // Arrange
      var state1 = new ContainingTypeState("MyType", true, false, ImmutableArray<GenericTypeParameterState>.Empty);
      var state2 = new ContainingTypeState("MyType", false, false, ImmutableArray<GenericTypeParameterState>.Empty);

      // Act
      var hashCode1 = state1.GetHashCode();
      var hashCode2 = state2.GetHashCode();

      // Assert
      hashCode1.Should().NotBe(hashCode2);
   }

   [Fact]
   public void Should_return_different_hash_code_for_different_isRecord()
   {
      // Arrange
      var state1 = new ContainingTypeState("MyType", true, false, ImmutableArray<GenericTypeParameterState>.Empty);
      var state2 = new ContainingTypeState("MyType", true, true, ImmutableArray<GenericTypeParameterState>.Empty);

      // Act
      var hashCode1 = state1.GetHashCode();
      var hashCode2 = state2.GetHashCode();

      // Assert
      hashCode1.Should().NotBe(hashCode2);
   }

   [Fact]
   public void Should_return_different_hash_code_for_different_generic_parameters()
   {
      // Arrange
      var genericParams1 = ImmutableArray.Create(
         new GenericTypeParameterState("T", ImmutableArray<string>.Empty));
      var genericParams2 = ImmutableArray.Create(
         new GenericTypeParameterState("TKey", ImmutableArray<string>.Empty));
      var state1 = new ContainingTypeState("MyClass", true, false, genericParams1);
      var state2 = new ContainingTypeState("MyClass", true, false, genericParams2);

      // Act
      var hashCode1 = state1.GetHashCode();
      var hashCode2 = state2.GetHashCode();

      // Assert
      hashCode1.Should().NotBe(hashCode2);
   }

   [Fact]
   public void Should_return_different_hash_code_for_different_generic_parameter_order()
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
      var hashCode1 = state1.GetHashCode();
      var hashCode2 = state2.GetHashCode();

      // Assert
      hashCode1.Should().NotBe(hashCode2);
   }

   [Fact]
   public void Should_handle_empty_name()
   {
      // Arrange
      var state = new ContainingTypeState("", true, false, ImmutableArray<GenericTypeParameterState>.Empty);

      // Act
      var hashCode = state.GetHashCode();

      // Assert - just verify it doesn't throw
      hashCode.Should().NotBe(0);
   }

   [Fact]
   public void Should_handle_empty_generic_parameters()
   {
      // Arrange
      var state = new ContainingTypeState("MyClass", true, false, ImmutableArray<GenericTypeParameterState>.Empty);

      // Act
      var hashCode = state.GetHashCode();

      // Assert - just verify it doesn't throw
      hashCode.Should().NotBe(0);
   }

   [Fact]
   public void Should_handle_single_generic_parameter()
   {
      // Arrange
      var genericParams = ImmutableArray.Create(
         new GenericTypeParameterState("T", ImmutableArray<string>.Empty));
      var state = new ContainingTypeState("MyClass", true, false, genericParams);

      // Act
      var hashCode = state.GetHashCode();

      // Assert - just verify it doesn't throw
      hashCode.Should().NotBe(0);
   }

   [Fact]
   public void Should_handle_multiple_generic_parameters()
   {
      // Arrange
      var genericParams = ImmutableArray.Create(
         new GenericTypeParameterState("T", ImmutableArray<string>.Empty),
         new GenericTypeParameterState("TKey", ImmutableArray<string>.Empty),
         new GenericTypeParameterState("TValue", ImmutableArray<string>.Empty));
      var state = new ContainingTypeState("MyClass", true, false, genericParams);

      // Act
      var hashCode = state.GetHashCode();

      // Assert - just verify it doesn't throw
      hashCode.Should().NotBe(0);
   }

   [Fact]
   public void Should_be_deterministic()
   {
      // Arrange
      var genericParams = ImmutableArray.Create(
         new GenericTypeParameterState("T", ImmutableArray<string>.Empty));
      var state = new ContainingTypeState("MyClass", true, false, genericParams);

      // Act
      var hashCode1 = state.GetHashCode();
      var hashCode2 = state.GetHashCode();
      var hashCode3 = state.GetHashCode();

      // Assert
      hashCode1.Should().Be(hashCode2);
      hashCode2.Should().Be(hashCode3);
   }

   [Fact]
   public void Should_satisfy_hash_code_contract_for_equal_objects()
   {
      // Arrange
      var genericParams = ImmutableArray.Create(new GenericTypeParameterState("T", ["class", "IDisposable"]));
      var state1 = new ContainingTypeState("MyClass", true, true, genericParams);
      var state2 = new ContainingTypeState("MyClass", true, true, genericParams);

      // Act & Assert
      state1.GetHashCode().Should().Be(state2.GetHashCode());
   }

   [Fact]
   public void Should_handle_value_type_instances()
   {
      // Arrange
      var state1 = new ContainingTypeState("MyStruct", false, false, ImmutableArray<GenericTypeParameterState>.Empty);
      var state2 = new ContainingTypeState("MyStruct", false, false, ImmutableArray<GenericTypeParameterState>.Empty);

      // Act
      var hashCode1 = state1.GetHashCode();
      var hashCode2 = state2.GetHashCode();

      // Assert
      hashCode1.Should().Be(hashCode2);
   }

   [Fact]
   public void Should_handle_record_instances()
   {
      // Arrange
      var state1 = new ContainingTypeState("MyRecord", true, true, ImmutableArray<GenericTypeParameterState>.Empty);
      var state2 = new ContainingTypeState("MyRecord", true, true, ImmutableArray<GenericTypeParameterState>.Empty);

      // Act
      var hashCode1 = state1.GetHashCode();
      var hashCode2 = state2.GetHashCode();

      // Assert
      hashCode1.Should().Be(hashCode2);
   }

   [Fact]
   public void Should_return_different_hash_code_for_empty_vs_non_empty_generic_parameters()
   {
      // Arrange
      var genericParams = ImmutableArray.Create(
         new GenericTypeParameterState("T", ImmutableArray<string>.Empty));
      var state1 = new ContainingTypeState("MyClass", true, false, ImmutableArray<GenericTypeParameterState>.Empty);
      var state2 = new ContainingTypeState("MyClass", true, false, genericParams);

      // Act
      var hashCode1 = state1.GetHashCode();
      var hashCode2 = state2.GetHashCode();

      // Assert
      hashCode1.Should().NotBe(hashCode2);
   }

   [Fact]
   public void Should_handle_all_boolean_flags_true()
   {
      // Arrange
      var state1 = new ContainingTypeState("MyRecord", true, true, ImmutableArray<GenericTypeParameterState>.Empty);
      var state2 = new ContainingTypeState("MyRecord", true, true, ImmutableArray<GenericTypeParameterState>.Empty);

      // Act
      var hashCode1 = state1.GetHashCode();
      var hashCode2 = state2.GetHashCode();

      // Assert
      hashCode1.Should().Be(hashCode2);
   }

   [Fact]
   public void Should_handle_all_boolean_flags_false()
   {
      // Arrange
      var state1 = new ContainingTypeState("MyStruct", false, false, ImmutableArray<GenericTypeParameterState>.Empty);
      var state2 = new ContainingTypeState("MyStruct", false, false, ImmutableArray<GenericTypeParameterState>.Empty);

      // Act
      var hashCode1 = state1.GetHashCode();
      var hashCode2 = state2.GetHashCode();

      // Assert
      hashCode1.Should().Be(hashCode2);
   }

   [Fact]
   public void Should_return_different_hash_code_when_all_boolean_flags_differ()
   {
      // Arrange
      var state1 = new ContainingTypeState("MyType", true, true, ImmutableArray<GenericTypeParameterState>.Empty);
      var state2 = new ContainingTypeState("MyType", false, false, ImmutableArray<GenericTypeParameterState>.Empty);

      // Act
      var hashCode1 = state1.GetHashCode();
      var hashCode2 = state2.GetHashCode();

      // Assert
      hashCode1.Should().NotBe(hashCode2);
   }

   [Fact]
   public void Should_handle_complex_equal_instances()
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
      var hashCode1 = state1.GetHashCode();
      var hashCode2 = state2.GetHashCode();

      // Assert
      hashCode1.Should().Be(hashCode2);
   }

   [Fact]
   public void Should_return_different_hash_code_for_different_generic_parameter_constraints()
   {
      // Arrange
      var genericParams1 = ImmutableArray.Create(
         new GenericTypeParameterState("T", ["class"]));
      var genericParams2 = ImmutableArray.Create(
         new GenericTypeParameterState("T", ["struct"]));
      var state1 = new ContainingTypeState("MyClass", true, false, genericParams1);
      var state2 = new ContainingTypeState("MyClass", true, false, genericParams2);

      // Act
      var hashCode1 = state1.GetHashCode();
      var hashCode2 = state2.GetHashCode();

      // Assert
      hashCode1.Should().NotBe(hashCode2);
   }
}

using System.Collections.Immutable;
using Thinktecture.CodeAnalysis;

namespace Thinktecture.Runtime.Tests.CodeAnalysis.GenericTypeParameterStateTests;

public class GetHashCode
{
   [Fact]
   public void Should_return_same_hash_code_for_same_instance()
   {
      // Arrange
      var state = new GenericTypeParameterState("T", ImmutableArray<string>.Empty);

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
      var constraints = ImmutableArray.Create("class", "IDisposable");
      var state1 = new GenericTypeParameterState("T", constraints);
      var state2 = new GenericTypeParameterState("T", constraints);

      // Act
      var hashCode1 = state1.GetHashCode();
      var hashCode2 = state2.GetHashCode();

      // Assert
      hashCode1.Should().Be(hashCode2);
   }

   [Fact]
   public void Should_return_same_hash_code_for_instances_with_equal_constraints()
   {
      // Arrange
      var constraints1 = ImmutableArray.Create("class", "IDisposable");
      var constraints2 = ImmutableArray.Create("class", "IDisposable");
      var state1 = new GenericTypeParameterState("T", constraints1);
      var state2 = new GenericTypeParameterState("T", constraints2);

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
      var state1 = new GenericTypeParameterState("T", ImmutableArray<string>.Empty);
      var state2 = new GenericTypeParameterState("TValue", ImmutableArray<string>.Empty);

      // Act
      var hashCode1 = state1.GetHashCode();
      var hashCode2 = state2.GetHashCode();

      // Assert
      hashCode1.Should().NotBe(hashCode2);
   }

   [Fact]
   public void Should_return_different_hash_code_for_different_constraints()
   {
      // Arrange
      var constraints1 = ImmutableArray.Create("class");
      var constraints2 = ImmutableArray.Create("struct");
      var state1 = new GenericTypeParameterState("T", constraints1);
      var state2 = new GenericTypeParameterState("T", constraints2);

      // Act
      var hashCode1 = state1.GetHashCode();
      var hashCode2 = state2.GetHashCode();

      // Assert
      hashCode1.Should().NotBe(hashCode2);
   }

   [Fact]
   public void Should_return_different_hash_code_for_different_constraint_order()
   {
      // Arrange
      var constraints1 = ImmutableArray.Create("class", "IDisposable");
      var constraints2 = ImmutableArray.Create("IDisposable", "class");
      var state1 = new GenericTypeParameterState("T", constraints1);
      var state2 = new GenericTypeParameterState("T", constraints2);

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
      var state = new GenericTypeParameterState("", ImmutableArray<string>.Empty);

      // Act
      var hashCode = state.GetHashCode();

      // Assert - just verify it doesn't throw
      hashCode.Should().NotBe(0);
   }

   [Fact]
   public void Should_handle_empty_constraints()
   {
      // Arrange
      var state = new GenericTypeParameterState("T", ImmutableArray<string>.Empty);

      // Act
      var hashCode = state.GetHashCode();

      // Assert - just verify it doesn't throw
      hashCode.Should().NotBe(0);
   }

   [Fact]
   public void Should_handle_single_constraint()
   {
      // Arrange
      var constraints = ImmutableArray.Create("class");
      var state = new GenericTypeParameterState("T", constraints);

      // Act
      var hashCode = state.GetHashCode();

      // Assert - just verify it doesn't throw
      hashCode.Should().NotBe(0);
   }

   [Fact]
   public void Should_handle_multiple_constraints()
   {
      // Arrange
      var constraints = ImmutableArray.Create("class", "IDisposable", "new()");
      var state = new GenericTypeParameterState("T", constraints);

      // Act
      var hashCode = state.GetHashCode();

      // Assert - just verify it doesn't throw
      hashCode.Should().NotBe(0);
   }

   [Fact]
   public void Should_be_deterministic()
   {
      // Arrange
      var constraints = ImmutableArray.Create("class", "IDisposable");
      var state = new GenericTypeParameterState("T", constraints);

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
      var constraints = ImmutableArray.Create("class", "IDisposable", "new()");
      var state1 = new GenericTypeParameterState("T", constraints);
      var state2 = new GenericTypeParameterState("T", constraints);

      // Act & Assert
      if (state1.Equals(state2))
      {
         state1.GetHashCode().Should().Be(state2.GetHashCode());
      }
   }
}

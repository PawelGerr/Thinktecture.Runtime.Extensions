using Thinktecture.CodeAnalysis;

#nullable enable

namespace Thinktecture.Runtime.Tests.CodeAnalysis.ValidationErrorStateTests;

public class Default
{
   [Fact]
   public void Should_have_correct_type_fully_qualified()
   {
      // Act
      var defaultState = ValidationErrorState.Default;

      // Assert
      defaultState.TypeFullyQualified.Should().Be("global::Thinktecture.ValidationError");
   }

   [Fact]
   public void Should_be_equal_to_instance_with_same_type()
   {
      // Arrange
      var manualState = new ValidationErrorState("global::Thinktecture.ValidationError");

      // Act
      var defaultState = ValidationErrorState.Default;

      // Assert
      defaultState.Should().Be(manualState);
   }

   [Fact]
   public void Should_not_be_equal_to_instance_with_different_type()
   {
      // Arrange
      var otherState = new ValidationErrorState("global::MyNamespace.MyValidationError");

      // Act
      var defaultState = ValidationErrorState.Default;

      // Assert
      defaultState.Should().NotBe(otherState);
   }

   [Fact]
   public void Should_always_return_same_instance()
   {
      // Act
      var first = ValidationErrorState.Default;
      var second = ValidationErrorState.Default;

      // Assert
      // For structs, this checks value equality, not reference equality
      first.Should().Be(second);
      first.TypeFullyQualified.Should().BeSameAs(second.TypeFullyQualified);
   }

   [Fact]
   public void Should_have_consistent_hash_code()
   {
      // Act
      var hashCode1 = ValidationErrorState.Default.GetHashCode();
      var hashCode2 = ValidationErrorState.Default.GetHashCode();

      // Assert
      hashCode1.Should().Be(hashCode2);
   }

   [Fact]
   public void Should_have_same_hash_code_as_manually_created_instance()
   {
      // Arrange
      var manualState = new ValidationErrorState("global::Thinktecture.ValidationError");

      // Act
      var defaultHashCode = ValidationErrorState.Default.GetHashCode();
      var manualHashCode = manualState.GetHashCode();

      // Assert
      defaultHashCode.Should().Be(manualHashCode);
   }
}

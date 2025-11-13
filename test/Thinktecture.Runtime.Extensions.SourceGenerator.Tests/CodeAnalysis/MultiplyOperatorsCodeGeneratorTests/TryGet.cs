using System;
using Thinktecture.CodeAnalysis;
using Thinktecture.CodeAnalysis.ValueObjects;

namespace Thinktecture.Runtime.Tests.CodeAnalysis.MultiplyOperatorsCodeGeneratorTests;

public class TryGet
{
   [Fact]
   public void Should_return_false_when_OperatorsGeneration_is_None()
   {
      // Arrange & Act
      var result = MultiplyOperatorsCodeGenerator.TryGet(
         ImplementedOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.None,
         out var generator);

      // Assert
      result.Should().BeFalse();
      generator.Should().BeNull();
   }

   [Fact]
   public void Should_return_singleton_for_Default_with_All_operators()
   {
      // Arrange & Act
      var result1 = MultiplyOperatorsCodeGenerator.TryGet(
         ImplementedOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         out var generator1);

      var result2 = MultiplyOperatorsCodeGenerator.TryGet(
         ImplementedOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         out var generator2);

      // Assert
      result1.Should().BeTrue();
      result2.Should().BeTrue();
      generator1.Should().NotBeNull();
      generator2.Should().NotBeNull();
      generator1.Should().BeSameAs(generator2, "should return the same singleton instance");
   }

   [Fact]
   public void Should_return_singleton_for_DefaultWithKeyTypeOverloads_with_All_operators()
   {
      // Arrange & Act
      var result1 = MultiplyOperatorsCodeGenerator.TryGet(
         ImplementedOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.DefaultWithKeyTypeOverloads,
         out var generator1);

      var result2 = MultiplyOperatorsCodeGenerator.TryGet(
         ImplementedOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.DefaultWithKeyTypeOverloads,
         out var generator2);

      // Assert
      result1.Should().BeTrue();
      result2.Should().BeTrue();
      generator1.Should().NotBeNull();
      generator2.Should().NotBeNull();
      generator1.Should().BeSameAs(generator2, "should return the same singleton instance");
   }

   [Fact]
   public void Should_return_different_singletons_for_Default_and_DefaultWithKeyTypeOverloads()
   {
      // Arrange & Act
      var result1 = MultiplyOperatorsCodeGenerator.TryGet(
         ImplementedOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         out var generator1);

      var result2 = MultiplyOperatorsCodeGenerator.TryGet(
         ImplementedOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.DefaultWithKeyTypeOverloads,
         out var generator2);

      // Assert
      result1.Should().BeTrue();
      result2.Should().BeTrue();
      generator1.Should().NotBeNull();
      generator2.Should().NotBeNull();
      generator1.Should().NotBeSameAs(generator2, "different generation modes should return different instances");
   }

   [Fact]
   public void Should_return_new_instance_when_partial_operators_specified()
   {
      // Arrange & Act
      var result1 = MultiplyOperatorsCodeGenerator.TryGet(
         ImplementedOperators.Default,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         out var generator1);

      var result2 = MultiplyOperatorsCodeGenerator.TryGet(
         ImplementedOperators.Default,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         out var generator2);

      // Assert
      result1.Should().BeTrue();
      result2.Should().BeTrue();
      generator1.Should().NotBeNull();
      generator2.Should().NotBeNull();
      generator1.Should().NotBeSameAs(generator2, "partial operators should create new instances");
   }

   [Fact]
   public void Should_return_new_instance_when_Checked_operator_only()
   {
      // Arrange & Act
      var result1 = MultiplyOperatorsCodeGenerator.TryGet(
         ImplementedOperators.Checked,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         out var generator1);

      var result2 = MultiplyOperatorsCodeGenerator.TryGet(
         ImplementedOperators.Checked,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         out var generator2);

      // Assert
      result1.Should().BeTrue();
      result2.Should().BeTrue();
      generator1.Should().NotBeNull();
      generator2.Should().NotBeNull();
      generator1.Should().NotBeSameAs(generator2, "checked-only operators should create new instances");
   }

   [Fact]
   public void Should_return_new_instances_for_same_partial_operators_with_key_type_overloads()
   {
      // Arrange & Act
      var result1 = MultiplyOperatorsCodeGenerator.TryGet(
         ImplementedOperators.Default,
         Thinktecture.CodeAnalysis.OperatorsGeneration.DefaultWithKeyTypeOverloads,
         out var generator1);

      var result2 = MultiplyOperatorsCodeGenerator.TryGet(
         ImplementedOperators.Default,
         Thinktecture.CodeAnalysis.OperatorsGeneration.DefaultWithKeyTypeOverloads,
         out var generator2);

      // Assert
      result1.Should().BeTrue();
      result2.Should().BeTrue();
      generator1.Should().NotBeNull();
      generator2.Should().NotBeNull();
      generator1.Should().NotBeSameAs(generator2, "partial operators with key type overloads should create new instances");
   }

   [Fact]
   public void Should_throw_ArgumentOutOfRangeException_for_invalid_enum_value()
   {
      // Arrange & Act
      var act = () => MultiplyOperatorsCodeGenerator.TryGet(
         ImplementedOperators.All,
         (Thinktecture.CodeAnalysis.OperatorsGeneration)999,
         out _);

      // Assert
      act.Should().Throw<ArgumentOutOfRangeException>()
         .WithMessage("*operatorsGeneration*")
         .WithMessage("*Invalid operations generation*");
   }

   [Theory]
   [InlineData(Thinktecture.CodeAnalysis.OperatorsGeneration.Default)]
   [InlineData(Thinktecture.CodeAnalysis.OperatorsGeneration.DefaultWithKeyTypeOverloads)]
   public void Should_return_true_for_valid_operations_generation(Thinktecture.CodeAnalysis.OperatorsGeneration operatorsGeneration)
   {
      // Arrange & Act
      var result = MultiplyOperatorsCodeGenerator.TryGet(
         ImplementedOperators.All,
         operatorsGeneration,
         out var generator);

      // Assert
      result.Should().BeTrue();
      generator.Should().NotBeNull();
   }

   [Theory]
   [InlineData(ImplementedOperators.Default)]
   [InlineData(ImplementedOperators.Checked)]
   [InlineData(ImplementedOperators.All)]
   public void Should_return_true_for_all_implemented_operators_variations(ImplementedOperators operators)
   {
      // Arrange & Act
      var result = MultiplyOperatorsCodeGenerator.TryGet(
         operators,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         out var generator);

      // Assert
      result.Should().BeTrue();
      generator.Should().NotBeNull();
   }

   [Fact]
   public void Should_have_correct_CodeGeneratorName()
   {
      // Arrange
      MultiplyOperatorsCodeGenerator.TryGet(
         ImplementedOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         out var generator);

      // Act & Assert
      generator!.CodeGeneratorName.Should().Be("MultiplyOperators-CodeGenerator");
   }

   [Fact]
   public void Should_have_correct_FileNameSuffix()
   {
      // Arrange
      MultiplyOperatorsCodeGenerator.TryGet(
         ImplementedOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         out var generator);

      // Act & Assert
      generator!.FileNameSuffix.Should().Be(".MultiplyOperators");
   }
}

using System;
using Thinktecture.CodeAnalysis;

namespace Thinktecture.Runtime.Tests.CodeAnalysis.ComparisonOperatorsCodeGeneratorTests;

public class TryGet
{
   [Fact]
   public void Should_return_false_when_OperatorsGeneration_is_None()
   {
      // Arrange & Act
      var result = ComparisonOperatorsCodeGenerator.TryGet(
         ImplementedComparisonOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.None,
         null,
         out var generator);

      // Assert
      result.Should().BeFalse();
      generator.Should().BeNull();
   }

   [Fact]
   public void Should_return_singleton_for_Default_with_null_comparer_and_All_operators()
   {
      // Arrange & Act
      var result1 = ComparisonOperatorsCodeGenerator.TryGet(
         ImplementedComparisonOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         null,
         out var generator1);

      var result2 = ComparisonOperatorsCodeGenerator.TryGet(
         ImplementedComparisonOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         null,
         out var generator2);

      // Assert
      result1.Should().BeTrue();
      result2.Should().BeTrue();
      generator1.Should().NotBeNull();
      generator2.Should().NotBeNull();
      generator1.Should().BeSameAs(generator2, "should return the same singleton instance");
   }

   [Fact]
   public void Should_return_singleton_for_DefaultWithKeyTypeOverloads_with_null_comparer_and_All_operators()
   {
      // Arrange & Act
      var result1 = ComparisonOperatorsCodeGenerator.TryGet(
         ImplementedComparisonOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.DefaultWithKeyTypeOverloads,
         null,
         out var generator1);

      var result2 = ComparisonOperatorsCodeGenerator.TryGet(
         ImplementedComparisonOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.DefaultWithKeyTypeOverloads,
         null,
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
      var result1 = ComparisonOperatorsCodeGenerator.TryGet(
         ImplementedComparisonOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         null,
         out var generator1);

      var result2 = ComparisonOperatorsCodeGenerator.TryGet(
         ImplementedComparisonOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.DefaultWithKeyTypeOverloads,
         null,
         out var generator2);

      // Assert
      result1.Should().BeTrue();
      result2.Should().BeTrue();
      generator1.Should().NotBeNull();
      generator2.Should().NotBeNull();
      generator1.Should().NotBeSameAs(generator2, "different generation modes should return different instances");
   }

   [Fact]
   public void Should_return_new_instance_when_custom_comparer_provided()
   {
      // Arrange & Act
      var result1 = ComparisonOperatorsCodeGenerator.TryGet(
         ImplementedComparisonOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         "MyComparerAccessor",
         out var generator1);

      var result2 = ComparisonOperatorsCodeGenerator.TryGet(
         ImplementedComparisonOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         "MyComparerAccessor",
         out var generator2);

      // Assert
      result1.Should().BeTrue();
      result2.Should().BeTrue();
      generator1.Should().NotBeNull();
      generator2.Should().NotBeNull();
      generator1.Should().NotBeSameAs(generator2, "custom comparer should create new instances");
   }

   [Fact]
   public void Should_return_new_instance_when_partial_operators_specified()
   {
      // Arrange & Act
      var result1 = ComparisonOperatorsCodeGenerator.TryGet(
         ImplementedComparisonOperators.LessThan,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         null,
         out var generator1);

      var result2 = ComparisonOperatorsCodeGenerator.TryGet(
         ImplementedComparisonOperators.LessThan,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         null,
         out var generator2);

      // Assert
      result1.Should().BeTrue();
      result2.Should().BeTrue();
      generator1.Should().NotBeNull();
      generator2.Should().NotBeNull();
      generator1.Should().NotBeSameAs(generator2, "partial operators should create new instances");
   }

   [Fact]
   public void Should_throw_ArgumentOutOfRangeException_for_invalid_enum_value()
   {
      // Arrange & Act
      var act = () => ComparisonOperatorsCodeGenerator.TryGet(
         ImplementedComparisonOperators.All,
         (Thinktecture.CodeAnalysis.OperatorsGeneration)999,
         null,
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
      var result = ComparisonOperatorsCodeGenerator.TryGet(
         ImplementedComparisonOperators.All,
         operatorsGeneration,
         null,
         out var generator);

      // Assert
      result.Should().BeTrue();
      generator.Should().NotBeNull();
   }

   [Fact]
   public void Should_have_correct_CodeGeneratorName()
   {
      // Arrange
      ComparisonOperatorsCodeGenerator.TryGet(
         ImplementedComparisonOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         null,
         out var generator);

      // Act & Assert
      generator!.CodeGeneratorName.Should().Be("ComparisonOperators-CodeGenerator");
   }

   [Fact]
   public void Should_have_correct_FileNameSuffix()
   {
      // Arrange
      ComparisonOperatorsCodeGenerator.TryGet(
         ImplementedComparisonOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         null,
         out var generator);

      // Act & Assert
      generator!.FileNameSuffix.Should().Be(".ComparisonOperators");
   }
}

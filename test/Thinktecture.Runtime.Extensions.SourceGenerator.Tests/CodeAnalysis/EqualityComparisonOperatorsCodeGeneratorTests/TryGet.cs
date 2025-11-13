using System;
using Thinktecture.CodeAnalysis;

namespace Thinktecture.Runtime.Tests.CodeAnalysis.EqualityComparisonOperatorsCodeGeneratorTests;

public class TryGet
{
   [Fact]
   public void WithOperatorsGenerationNone_ReturnsFalse_AndGeneratorIsNull()
   {
      // Arrange
      var operatorsGeneration = Thinktecture.CodeAnalysis.OperatorsGeneration.None;

      // Act
      var result = EqualityComparisonOperatorsCodeGenerator.TryGet(
         operatorsGeneration,
         null,
         out var generator);

      // Assert
      result.Should().BeFalse();
      generator.Should().BeNull();
   }

   [Fact]
   public void WithOperatorsGenerationDefault_AndNullComparer_ReturnsTrue_AndCachedInstance()
   {
      // Arrange
      var operatorsGeneration = Thinktecture.CodeAnalysis.OperatorsGeneration.Default;

      // Act
      var result1 = EqualityComparisonOperatorsCodeGenerator.TryGet(
         operatorsGeneration,
         null,
         out var generator1);

      var result2 = EqualityComparisonOperatorsCodeGenerator.TryGet(
         operatorsGeneration,
         null,
         out var generator2);

      // Assert
      result1.Should().BeTrue();
      result2.Should().BeTrue();
      generator1.Should().NotBeNull();
      generator2.Should().NotBeNull();
      generator1.Should().BeSameAs(generator2); // Same cached instance
   }

   [Fact]
   public void WithOperatorsGenerationDefault_AndComparer_ReturnsTrue_AndNewInstance()
   {
      // Arrange
      var operatorsGeneration = Thinktecture.CodeAnalysis.OperatorsGeneration.Default;
      var equalityComparer = new ComparerInfo("TestComparer", IsAccessor: true);

      // Act
      var result1 = EqualityComparisonOperatorsCodeGenerator.TryGet(
         operatorsGeneration,
         equalityComparer,
         out var generator1);

      var result2 = EqualityComparisonOperatorsCodeGenerator.TryGet(
         operatorsGeneration,
         equalityComparer,
         out var generator2);

      // Assert
      result1.Should().BeTrue();
      result2.Should().BeTrue();
      generator1.Should().NotBeNull();
      generator2.Should().NotBeNull();
      generator1.Should().NotBeSameAs(generator2); // Different instances
   }

   [Fact]
   public void WithOperatorsGenerationDefaultWithKeyTypeOverloads_AndNullComparer_ReturnsTrue_AndCachedInstance()
   {
      // Arrange
      var operatorsGeneration = Thinktecture.CodeAnalysis.OperatorsGeneration.DefaultWithKeyTypeOverloads;

      // Act
      var result1 = EqualityComparisonOperatorsCodeGenerator.TryGet(
         operatorsGeneration,
         null,
         out var generator1);

      var result2 = EqualityComparisonOperatorsCodeGenerator.TryGet(
         operatorsGeneration,
         null,
         out var generator2);

      // Assert
      result1.Should().BeTrue();
      result2.Should().BeTrue();
      generator1.Should().NotBeNull();
      generator2.Should().NotBeNull();
      generator1.Should().BeSameAs(generator2); // Same cached instance
   }

   [Fact]
   public void WithOperatorsGenerationDefaultWithKeyTypeOverloads_AndComparer_ReturnsTrue_AndNewInstance()
   {
      // Arrange
      var operatorsGeneration = Thinktecture.CodeAnalysis.OperatorsGeneration.DefaultWithKeyTypeOverloads;
      var equalityComparer = new ComparerInfo("CustomComparer", IsAccessor: false);

      // Act
      var result1 = EqualityComparisonOperatorsCodeGenerator.TryGet(
         operatorsGeneration,
         equalityComparer,
         out var generator1);

      var result2 = EqualityComparisonOperatorsCodeGenerator.TryGet(
         operatorsGeneration,
         equalityComparer,
         out var generator2);

      // Assert
      result1.Should().BeTrue();
      result2.Should().BeTrue();
      generator1.Should().NotBeNull();
      generator2.Should().NotBeNull();
      generator1.Should().NotBeSameAs(generator2); // Different instances
   }

   [Fact]
   public void WithInvalidOperatorsGeneration_ThrowsArgumentOutOfRangeException()
   {
      // Arrange
      var operatorsGeneration = (Thinktecture.CodeAnalysis.OperatorsGeneration)999;

      // Act & Assert
      var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
         EqualityComparisonOperatorsCodeGenerator.TryGet(
            operatorsGeneration,
            null,
            out _));

      exception.ParamName.Should().Be("operatorsGeneration");
      exception.Message.Should().Contain("Invalid operations generation");
   }

   [Fact]
   public void WithDefaultVsDefaultWithKeyTypeOverloads_ReturnsDifferentCachedInstances()
   {
      // Arrange & Act
      EqualityComparisonOperatorsCodeGenerator.TryGet(
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         null,
         out var defaultGenerator);

      EqualityComparisonOperatorsCodeGenerator.TryGet(
         Thinktecture.CodeAnalysis.OperatorsGeneration.DefaultWithKeyTypeOverloads,
         null,
         out var withKeyTypeOverloadsGenerator);

      // Assert
      defaultGenerator.Should().NotBeNull();
      withKeyTypeOverloadsGenerator.Should().NotBeNull();
      defaultGenerator.Should().NotBeSameAs(withKeyTypeOverloadsGenerator);
   }

   [Fact]
   public void WithSameComparerValue_ButDifferentInstances_CreatesNewGenerators()
   {
      // Arrange
      var comparer1 = new ComparerInfo("TestComparer", IsAccessor: true);
      var comparer2 = new ComparerInfo("TestComparer", IsAccessor: true);

      // Act
      EqualityComparisonOperatorsCodeGenerator.TryGet(
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         comparer1,
         out var generator1);

      EqualityComparisonOperatorsCodeGenerator.TryGet(
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         comparer2,
         out var generator2);

      // Assert
      generator1.Should().NotBeNull();
      generator2.Should().NotBeNull();
      generator1.Should().NotBeSameAs(generator2); // Even with same comparer values, creates new instances
   }
}

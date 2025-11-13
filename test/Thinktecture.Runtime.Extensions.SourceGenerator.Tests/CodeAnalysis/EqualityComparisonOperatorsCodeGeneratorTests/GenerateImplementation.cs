using System.Text;
using System.Threading.Tasks;
using Thinktecture.CodeAnalysis;
using VerifyXunit;

namespace Thinktecture.Runtime.Tests.CodeAnalysis.EqualityComparisonOperatorsCodeGeneratorTests;

public class GenerateImplementation
{
   [Fact]
   public void GenerateImplementation_WithEmptyStringBuilder_AppendsContent()
   {
      // Arrange
      var state = new EqualityComparisonOperatorsStateBuilder()
                  .WithIntKeyMember()
                  .Build();

      EqualityComparisonOperatorsCodeGenerator.TryGet(
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         null,
         out var generator);

      var sb = new StringBuilder();

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      sb.Length.Should().BeGreaterThan(0);
   }

   [Fact]
   public async Task WithReferenceType_AndReferenceEquality_UsesReferenceEquals()
   {
      // Arrange
      var state = new EqualityComparisonOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.MyClass", "MyClass", isEqualWithReferenceEquality: true)
                  .WithIntKeyMember("_value")
                  .Build();

      EqualityComparisonOperatorsCodeGenerator.TryGet(
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         null,
         out var generator);

      var sb = new StringBuilder();

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("return global::System.Object.ReferenceEquals(obj, other);");
      result.Should().NotContain("if (obj is null)");
      result.Should().NotContain("obj.Equals(other)");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task WithReferenceType_AndNoReferenceEquality_UsesNullChecksAndEquals()
   {
      // Arrange
      var state = new EqualityComparisonOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.ProductId", "ProductId", isEqualWithReferenceEquality: false)
                  .WithStringKeyMember("_value")
                  .Build();

      EqualityComparisonOperatorsCodeGenerator.TryGet(
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         null,
         out var generator);

      var sb = new StringBuilder();

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("if (obj is null)");
      result.Should().Contain("return other is null;");
      result.Should().Contain("return obj.Equals(other);");
      result.Should().NotContain("ReferenceEquals");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task WithValueType_UsesEqualsDirectly()
   {
      // Arrange
      var state = new EqualityComparisonOperatorsStateBuilder()
                  .WithValueType("global::Thinktecture.Tests.Amount", "Amount")
                  .WithIntKeyMember("_value")
                  .Build();

      EqualityComparisonOperatorsCodeGenerator.TryGet(
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         null,
         out var generator);

      var sb = new StringBuilder();

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("return obj.Equals(other);");
      result.Should().NotContain("if (obj is null)");
      result.Should().NotContain("ReferenceEquals");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task GeneratesInequalityOperator_UsingEqualityOperator()
   {
      // Arrange
      var state = new EqualityComparisonOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.TestType")
                  .WithIntKeyMember()
                  .Build();

      EqualityComparisonOperatorsCodeGenerator.TryGet(
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         null,
         out var generator);

      var sb = new StringBuilder();

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("public static bool operator !=");
      result.Should().Contain("return !(obj == other);");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task IncludesXmlDocumentation_ForOperators()
   {
      // Arrange
      var state = new EqualityComparisonOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.MyType", "MyType")
                  .WithIntKeyMember()
                  .Build();

      EqualityComparisonOperatorsCodeGenerator.TryGet(
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         null,
         out var generator);

      var sb = new StringBuilder();

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("/// <summary>");
      result.Should().Contain("/// Compares two instances of");
      result.Should().Contain("/// <param name=\"obj\">Instance to compare.</param>");
      result.Should().Contain("/// <param name=\"other\">Another instance to compare.</param>");
      result.Should().Contain("/// <returns><c>true</c> if objects are equal; otherwise <c>false</c>.</returns>");
      result.Should().Contain("/// <returns><c>false</c> if objects are equal; otherwise <c>true</c>.</returns>");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task WithComplexGenericTypeName_GeneratesCorrectly()
   {
      // Arrange
      var state = new EqualityComparisonOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.Container<global::System.String>.Inner", "Inner")
                  .WithGuidKeyMember("_id")
                  .Build();

      EqualityComparisonOperatorsCodeGenerator.TryGet(
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         null,
         out var generator);

      var sb = new StringBuilder();

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task WithSpecialCharactersInTypeName_EscapesCorrectly()
   {
      // Arrange
      var state = new EqualityComparisonOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.@Special.@Type", "Type")
                  .WithIntKeyMember("@value")
                  .Build();

      EqualityComparisonOperatorsCodeGenerator.TryGet(
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         null,
         out var generator);

      var sb = new StringBuilder();

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task EqualityOperatorSignature_IncludesNullableAnnotations()
   {
      // Arrange
      var state = new EqualityComparisonOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.ProductId", "ProductId")
                  .WithStringKeyMember("Value")
                  .Build();

      EqualityComparisonOperatorsCodeGenerator.TryGet(
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         null,
         out var generator);

      var sb = new StringBuilder();

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("public static bool operator ==(global::Thinktecture.Tests.ProductId? obj, global::Thinktecture.Tests.ProductId? other)");
      result.Should().Contain("public static bool operator !=(global::Thinktecture.Tests.ProductId? obj, global::Thinktecture.Tests.ProductId? other)");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task WithValueType_SignatureDoesNotIncludeNullable()
   {
      // Arrange
      var state = new EqualityComparisonOperatorsStateBuilder()
                  .WithValueType("global::Thinktecture.Tests.Amount", "Amount")
                  .WithIntKeyMember()
                  .Build();

      EqualityComparisonOperatorsCodeGenerator.TryGet(
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         null,
         out var generator);

      var sb = new StringBuilder();

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("public static bool operator ==(global::Thinktecture.Tests.Amount obj, global::Thinktecture.Tests.Amount other)");
      result.Should().Contain("public static bool operator !=(global::Thinktecture.Tests.Amount obj, global::Thinktecture.Tests.Amount other)");
      result.Should().NotContain("Amount?");
      await Verifier.Verify(result);
   }
}

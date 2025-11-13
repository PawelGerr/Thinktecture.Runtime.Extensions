using System.Text;
using System.Threading.Tasks;
using Thinktecture.CodeAnalysis;
using VerifyXunit;

namespace Thinktecture.Runtime.Tests.CodeAnalysis.EqualityComparisonOperatorsCodeGeneratorTests;

public class GenerateBaseTypes
{
   [Fact]
   public async Task WithDefault_GeneratesOnlyBasicInterface()
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
      generator!.GenerateBaseTypes(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("global::System.Numerics.IEqualityOperators<global::Thinktecture.Tests.TestType, global::Thinktecture.Tests.TestType, bool>");
      result.Should().NotContain("global::System.Int32"); // Should not include key member type
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task WithDefaultWithKeyTypeOverloads_AndNoKeyMember_GeneratesOnlyBasicInterface()
   {
      // Arrange
      var state = new EqualityComparisonOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.TestType")
                  .WithoutKeyMember()
                  .Build();

      EqualityComparisonOperatorsCodeGenerator.TryGet(
         Thinktecture.CodeAnalysis.OperatorsGeneration.DefaultWithKeyTypeOverloads,
         null,
         out var generator);

      var sb = new StringBuilder();

      // Act
      generator!.GenerateBaseTypes(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("global::System.Numerics.IEqualityOperators<global::Thinktecture.Tests.TestType, global::Thinktecture.Tests.TestType, bool>");
      result.Should().NotContain(",\n   global::System.Numerics.IEqualityOperators"); // Should not have second interface
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task WithDefaultWithKeyTypeOverloads_AndKeyMember_GeneratesBothInterfaces()
   {
      // Arrange
      var state = new EqualityComparisonOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.ProductId", "ProductId")
                  .WithStringKeyMember("_value")
                  .Build();

      EqualityComparisonOperatorsCodeGenerator.TryGet(
         Thinktecture.CodeAnalysis.OperatorsGeneration.DefaultWithKeyTypeOverloads,
         null,
         out var generator);

      var sb = new StringBuilder();

      // Act
      generator!.GenerateBaseTypes(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("global::System.Numerics.IEqualityOperators<global::Thinktecture.Tests.ProductId, global::Thinktecture.Tests.ProductId, bool>");
      result.Should().Contain("global::System.Numerics.IEqualityOperators<global::Thinktecture.Tests.ProductId, global::System.String, bool>");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task WithValueType_AndKeyTypeOverloads_GeneratesCorrectInterfaces()
   {
      // Arrange
      var state = new EqualityComparisonOperatorsStateBuilder()
                  .WithValueType("global::Thinktecture.Tests.Amount", "Amount")
                  .WithIntKeyMember("Value")
                  .Build();

      EqualityComparisonOperatorsCodeGenerator.TryGet(
         Thinktecture.CodeAnalysis.OperatorsGeneration.DefaultWithKeyTypeOverloads,
         null,
         out var generator);

      var sb = new StringBuilder();

      // Act
      generator!.GenerateBaseTypes(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("global::System.Numerics.IEqualityOperators<global::Thinktecture.Tests.Amount, global::Thinktecture.Tests.Amount, bool>");
      result.Should().Contain("global::System.Numerics.IEqualityOperators<global::Thinktecture.Tests.Amount, global::System.Int32, bool>");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task WithComplexGenericType_GeneratesCorrectlyEscapedInterfaces()
   {
      // Arrange
      var state = new EqualityComparisonOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.Container<global::System.String>.Inner", "Inner")
                  .WithGuidKeyMember("_id")
                  .Build();

      EqualityComparisonOperatorsCodeGenerator.TryGet(
         Thinktecture.CodeAnalysis.OperatorsGeneration.DefaultWithKeyTypeOverloads,
         null,
         out var generator);

      var sb = new StringBuilder();

      // Act
      generator!.GenerateBaseTypes(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("global::System.Numerics.IEqualityOperators<global::Thinktecture.Tests.Container<global::System.String>.Inner, global::Thinktecture.Tests.Container<global::System.String>.Inner, bool>");
      result.Should().Contain("global::System.Numerics.IEqualityOperators<global::Thinktecture.Tests.Container<global::System.String>.Inner, global::System.Guid, bool>");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task WithCustomKeyType_GeneratesCorrectKeyOverloadInterface()
   {
      // Arrange
      var state = new EqualityComparisonOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.Order", "Order")
                  .WithKeyMember("OrderId", "global::Thinktecture.Tests.OrderId", isReferenceType: true)
                  .Build();

      EqualityComparisonOperatorsCodeGenerator.TryGet(
         Thinktecture.CodeAnalysis.OperatorsGeneration.DefaultWithKeyTypeOverloads,
         null,
         out var generator);

      var sb = new StringBuilder();

      // Act
      generator!.GenerateBaseTypes(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("global::System.Numerics.IEqualityOperators<global::Thinktecture.Tests.Order, global::Thinktecture.Tests.Order, bool>");
      result.Should().Contain("global::System.Numerics.IEqualityOperators<global::Thinktecture.Tests.Order, global::Thinktecture.Tests.OrderId, bool>");
      await Verifier.Verify(result);
   }

   [Fact]
   public void GenerateBaseTypes_WithEmptyStringBuilder_AppendsContent()
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
      generator!.GenerateBaseTypes(sb, state);

      // Assert
      sb.Length.Should().BeGreaterThan(0);
   }
}

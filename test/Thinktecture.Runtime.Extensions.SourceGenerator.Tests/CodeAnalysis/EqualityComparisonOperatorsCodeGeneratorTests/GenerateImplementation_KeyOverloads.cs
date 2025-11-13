using System.Text;
using System.Threading.Tasks;
using Thinktecture.CodeAnalysis;
using VerifyXunit;

namespace Thinktecture.Runtime.Tests.CodeAnalysis.EqualityComparisonOperatorsCodeGeneratorTests;

// ReSharper disable once InconsistentNaming
public class GenerateImplementation_KeyOverloads
{
   [Fact]
   public void WithDefault_AndNoKeyTypeOverloads_DoesNotGenerateKeyOverloads()
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
      result.Should().NotContain("private static bool Equals");
      result.Should().NotContain("global::System.Int32 value");
   }

   [Fact]
   public void WithKeyTypeOverloads_AndNoKeyMember_DoesNotGenerateKeyOverloads()
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
      generator!.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().NotContain("private static bool Equals");
   }

   [Fact]
   public async Task WithReferenceType_AndReferenceTypeKey_GeneratesNullChecksForBoth()
   {
      // Arrange
      var state = new EqualityComparisonOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.Order", "Order")
                  .WithStringKeyMember("_orderId")
                  .Build();

      EqualityComparisonOperatorsCodeGenerator.TryGet(
         Thinktecture.CodeAnalysis.OperatorsGeneration.DefaultWithKeyTypeOverloads,
         null,
         out var generator);

      var sb = new StringBuilder();

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("if (obj is null)");
      result.Should().Contain("return value is null;");
      result.Should().Contain("if(value is null)");
      result.Should().Contain("return false;");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task WithReferenceType_AndValueTypeKey_GeneratesNullCheckOnlyForObj()
   {
      // Arrange
      var state = new EqualityComparisonOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.ProductId", "ProductId")
                  .WithIntKeyMember("_value")
                  .Build();

      EqualityComparisonOperatorsCodeGenerator.TryGet(
         Thinktecture.CodeAnalysis.OperatorsGeneration.DefaultWithKeyTypeOverloads,
         null,
         out var generator);

      var sb = new StringBuilder();

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("if (obj is null)");
      result.Should().Contain("return false;");
      // Should not have a separate null check for value since it's a value type
      var nullCheckCount = System.Text.RegularExpressions.Regex.Matches(result, @"if\s*\(\s*obj\s+is\s+null\s*\)").Count;
      nullCheckCount.Should().Be(2);
      result.Should().NotContain("value is null");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task WithValueType_AndReferenceTypeKey_DoesNotGenerateNullCheckForObj()
   {
      // Arrange
      var state = new EqualityComparisonOperatorsStateBuilder()
                  .WithValueType("global::Thinktecture.Tests.Amount", "Amount")
                  .WithStringKeyMember("Currency")
                  .Build();

      EqualityComparisonOperatorsCodeGenerator.TryGet(
         Thinktecture.CodeAnalysis.OperatorsGeneration.DefaultWithKeyTypeOverloads,
         null,
         out var generator);

      var sb = new StringBuilder();

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().NotContain("if (obj is null)");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task WithValueType_AndValueTypeKey_DoesNotGenerateNullChecks()
   {
      // Arrange
      var state = new EqualityComparisonOperatorsStateBuilder()
                  .WithValueType("global::Thinktecture.Tests.Price", "Price")
                  .WithIntKeyMember("Value")
                  .Build();

      EqualityComparisonOperatorsCodeGenerator.TryGet(
         Thinktecture.CodeAnalysis.OperatorsGeneration.DefaultWithKeyTypeOverloads,
         null,
         out var generator);

      var sb = new StringBuilder();

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().NotContain("if (obj is null)");
      result.Should().NotContain("if(value is null)");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task WithCustomEqualityComparer_AsAccessor_UsesComparerEqualityComparer()
   {
      // Arrange
      var state = new EqualityComparisonOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.ProductGroup", "ProductGroup")
                  .WithStringKeyMember("_key")
                  .Build();

      EqualityComparisonOperatorsCodeGenerator.TryGet(
         Thinktecture.CodeAnalysis.OperatorsGeneration.DefaultWithKeyTypeOverloads,
         new ComparerInfo("global::Thinktecture.Tests.ProductGroup.KeyMemberComparer", IsAccessor: true),
         out var generator);

      var sb = new StringBuilder();

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("global::Thinktecture.Tests.ProductGroup.KeyMemberComparer.EqualityComparer.Equals(obj._key, value)");
      result.Should().NotContain("StringComparer");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task WithCustomEqualityComparer_NotAccessor_UsesComparerDirectly()
   {
      // Arrange
      var state = new EqualityComparisonOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.CustomType", "CustomType")
                  .WithIntKeyMember("Value")
                  .Build();

      EqualityComparisonOperatorsCodeGenerator.TryGet(
         Thinktecture.CodeAnalysis.OperatorsGeneration.DefaultWithKeyTypeOverloads,
         new ComparerInfo("CustomComparer", IsAccessor: false),
         out var generator);

      var sb = new StringBuilder();

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("CustomComparer.Equals(obj.Value, value)");
      result.Should().NotContain(".EqualityComparer");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task WithStringKey_AndNoComparer_UsesStringComparerOrdinalIgnoreCase()
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
      generator!.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("global::System.StringComparer.OrdinalIgnoreCase.Equals(obj._value, value)");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task WithReferenceTypeKey_AndNoComparer_UsesNullConditionalAndEquals()
   {
      // Arrange
      var state = new EqualityComparisonOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.Order", "Order")
                  .WithKeyMember("CustomerId", "global::Thinktecture.Tests.CustomerId", isReferenceType: true)
                  .Build();

      EqualityComparisonOperatorsCodeGenerator.TryGet(
         Thinktecture.CodeAnalysis.OperatorsGeneration.DefaultWithKeyTypeOverloads,
         null,
         out var generator);

      var sb = new StringBuilder();

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("obj.CustomerId is null ? value is null : obj.CustomerId.Equals(value)");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task WithValueTypeKey_AndNoComparer_UsesEqualsDirectly()
   {
      // Arrange
      var state = new EqualityComparisonOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.Timestamp", "Timestamp")
                  .WithIntKeyMember("Ticks")
                  .Build();

      EqualityComparisonOperatorsCodeGenerator.TryGet(
         Thinktecture.CodeAnalysis.OperatorsGeneration.DefaultWithKeyTypeOverloads,
         null,
         out var generator);

      var sb = new StringBuilder();

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("obj.Ticks.Equals(value)");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task GeneratesBothDirectionsOfEqualityOperator()
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
      generator!.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      // obj == value
      result.Should().Contain("public static bool operator ==(global::Thinktecture.Tests.ProductId? obj, global::System.String? value)");
      // value == obj
      result.Should().Contain("public static bool operator ==(global::System.String? value, global::Thinktecture.Tests.ProductId? obj)");
      // Both should call the Equals method
      result.Should().Contain("return Equals(obj, value);");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task GeneratesBothDirectionsOfInequalityOperator()
   {
      // Arrange
      var state = new EqualityComparisonOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.Amount", "Amount")
                  .WithIntKeyMember("Value")
                  .Build();

      EqualityComparisonOperatorsCodeGenerator.TryGet(
         Thinktecture.CodeAnalysis.OperatorsGeneration.DefaultWithKeyTypeOverloads,
         null,
         out var generator);

      var sb = new StringBuilder();

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      // obj != value
      result.Should().Contain("public static bool operator !=(global::Thinktecture.Tests.Amount? obj, global::System.Int32 value)");
      // value != obj
      result.Should().Contain("public static bool operator !=(global::System.Int32 value, global::Thinktecture.Tests.Amount? obj)");
      // Both should use !(obj == value)
      result.Should().Contain("return !(obj == value);");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task IncludesXmlDocumentation_ForKeyOverloadOperators()
   {
      // Arrange
      var state = new EqualityComparisonOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.ProductId", "ProductId")
                  .WithStringKeyMember("Value")
                  .Build();

      EqualityComparisonOperatorsCodeGenerator.TryGet(
         Thinktecture.CodeAnalysis.OperatorsGeneration.DefaultWithKeyTypeOverloads,
         null,
         out var generator);

      var sb = new StringBuilder();

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("/// <summary>");
      result.Should().Contain("/// Compares an instance of");
      result.Should().Contain("/// <param name=\"obj\">Instance to compare.</param>");
      result.Should().Contain("/// <param name=\"value\">Value to compare");
      result.Should().Contain("/// <returns><c>true</c> if objects are equal; otherwise <c>false</c>.</returns>");
      result.Should().Contain("/// <returns><c>false</c> if objects are equal; otherwise <c>true</c>.</returns>");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task WithGuidKey_GeneratesCorrectOperators()
   {
      // Arrange
      var state = new EqualityComparisonOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.OrderId", "OrderId")
                  .WithGuidKeyMember("_id")
                  .Build();

      EqualityComparisonOperatorsCodeGenerator.TryGet(
         Thinktecture.CodeAnalysis.OperatorsGeneration.DefaultWithKeyTypeOverloads,
         null,
         out var generator);

      var sb = new StringBuilder();

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("obj._id.Equals(value)");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task WithComplexGenericType_GeneratesCorrectly()
   {
      // Arrange
      var state = new EqualityComparisonOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.Container<global::System.String>.Inner", "Inner")
                  .WithIntKeyMember("Id")
                  .Build();

      EqualityComparisonOperatorsCodeGenerator.TryGet(
         Thinktecture.CodeAnalysis.OperatorsGeneration.DefaultWithKeyTypeOverloads,
         null,
         out var generator);

      var sb = new StringBuilder();

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task PrivateEqualsMethod_IsGenerated()
   {
      // Arrange
      var state = new EqualityComparisonOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.TestType")
                  .WithIntKeyMember()
                  .Build();

      EqualityComparisonOperatorsCodeGenerator.TryGet(
         Thinktecture.CodeAnalysis.OperatorsGeneration.DefaultWithKeyTypeOverloads,
         null,
         out var generator);

      var sb = new StringBuilder();

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("private static bool Equals(global::Thinktecture.Tests.TestType? obj, global::System.Int32 value)");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task WithDifferentKeyMemberNames_GeneratesCorrectly()
   {
      // Arrange
      var state = new EqualityComparisonOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.TestType")
                  .WithIntKeyMember("CustomKeyName")
                  .Build();

      EqualityComparisonOperatorsCodeGenerator.TryGet(
         Thinktecture.CodeAnalysis.OperatorsGeneration.DefaultWithKeyTypeOverloads,
         null,
         out var generator);

      var sb = new StringBuilder();

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("obj.CustomKeyName.Equals(value)");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task WithValueTypeStruct_AndStringKey_GeneratesCorrectNullableAnnotations()
   {
      // Arrange
      var state = new EqualityComparisonOperatorsStateBuilder()
                  .WithValueType("global::Thinktecture.Tests.MyStruct", "MyStruct")
                  .WithStringKeyMember("Name")
                  .Build();

      EqualityComparisonOperatorsCodeGenerator.TryGet(
         Thinktecture.CodeAnalysis.OperatorsGeneration.DefaultWithKeyTypeOverloads,
         null,
         out var generator);

      var sb = new StringBuilder();

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      // Struct doesn't have nullable annotation in signature
      result.Should().Contain("private static bool Equals(global::Thinktecture.Tests.MyStruct obj, global::System.String? value)");
      result.Should().Contain("public static bool operator ==(global::Thinktecture.Tests.MyStruct obj, global::System.String? value)");
      await Verifier.Verify(result);
   }
}

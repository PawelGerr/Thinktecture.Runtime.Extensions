using System.Text;
using System.Threading.Tasks;
using Thinktecture.CodeAnalysis;
using VerifyXunit;

namespace Thinktecture.Runtime.Tests.CodeAnalysis.ComparisonOperatorsCodeGeneratorTests;

public class GenerateImplementation
{
   [Fact]
   public async Task UsingOperators_AllOperators_ReferenceType_GeneratesAllOperatorsWithNullChecks()
   {
      // Arrange
      var state = new ComparisonOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.MyClass", "MyClass")
                  .WithIntKeyMember("_value")
                  .Build();

      var sb = new StringBuilder();

      ComparisonOperatorsCodeGenerator.TryGet(
         ImplementedComparisonOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         null,
         out var generator);

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("operator <");
      result.Should().Contain("operator <=");
      result.Should().Contain("operator >");
      result.Should().Contain("operator >=");
      result.Should().Contain("global::System.ArgumentNullException.ThrowIfNull(left)");
      result.Should().Contain("global::System.ArgumentNullException.ThrowIfNull(right)");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task UsingOperators_AllOperators_ValueType_GeneratesAllOperatorsWithoutNullChecks()
   {
      // Arrange
      var state = new ComparisonOperatorsStateBuilder()
                  .WithValueType("global::Thinktecture.Tests.MyStruct", "MyStruct")
                  .WithIntKeyMember("_value")
                  .Build();

      var sb = new StringBuilder();

      ComparisonOperatorsCodeGenerator.TryGet(
         ImplementedComparisonOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         null,
         out var generator);

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("operator <");
      result.Should().Contain("operator <=");
      result.Should().Contain("operator >");
      result.Should().Contain("operator >=");
      result.Should().NotContain("ArgumentNullException");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task UsingOperators_OnlyLessThan_GeneratesOnlyLessThanOperator()
   {
      // Arrange
      var state = new ComparisonOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.TestType", "TestType")
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();

      ComparisonOperatorsCodeGenerator.TryGet(
         ImplementedComparisonOperators.LessThan,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         null,
         out var generator);

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("operator <");
      result.Should().NotContain("operator <=");
      result.Should().NotContain("operator >");
      result.Should().NotContain("operator >=");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task UsingOperators_OnlyGreaterThan_GeneratesOnlyGreaterThanOperator()
   {
      // Arrange
      var state = new ComparisonOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.TestType", "TestType")
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();

      ComparisonOperatorsCodeGenerator.TryGet(
         ImplementedComparisonOperators.GreaterThan,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         null,
         out var generator);

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().NotContain("operator <");
      result.Should().NotContain("operator <=");
      result.Should().Contain("operator >");
      result.Should().NotContain("operator >=");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task UsingOperators_LessThanAndLessThanOrEqual_GeneratesBothOperators()
   {
      // Arrange
      var state = new ComparisonOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.TestType", "TestType")
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();

      ComparisonOperatorsCodeGenerator.TryGet(
         ImplementedComparisonOperators.LessThan | ImplementedComparisonOperators.LessThanOrEqual,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         null,
         out var generator);

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("operator <");
      result.Should().Contain("operator <=");
      result.Should().NotContain("operator >");
      result.Should().NotContain("operator >=");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task UsingOperators_GreaterThanAndGreaterThanOrEqual_GeneratesBothOperators()
   {
      // Arrange
      var state = new ComparisonOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.TestType", "TestType")
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();

      ComparisonOperatorsCodeGenerator.TryGet(
         ImplementedComparisonOperators.GreaterThan | ImplementedComparisonOperators.GreaterThanOrEqual,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         null,
         out var generator);

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().NotContain("operator <");
      result.Should().NotContain("operator <=");
      result.Should().Contain("operator >");
      result.Should().Contain("operator >=");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task UsingOperators_MixedNullability_LeftReferenceRightValue_GeneratesCorrectNullChecks()
   {
      // Arrange
      var state = new ComparisonOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.MyClass", "MyClass")
                  .WithIntKeyMember("Value", isReferenceType: false)
                  .Build();

      var sb = new StringBuilder();

      ComparisonOperatorsCodeGenerator.TryGet(
         ImplementedComparisonOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         null,
         out var generator);

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("global::System.ArgumentNullException.ThrowIfNull(left)");
      result.Should().Contain("global::System.ArgumentNullException.ThrowIfNull(right)");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task UsingComparer_ReferenceType_GeneratesAllOperatorsUsingComparer()
   {
      // Arrange
      var state = new ComparisonOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.MyClass", "MyClass")
                  .WithKeyMember("_value", "global::System.DateTime", isReferenceType: false)
                  .Build();

      var sb = new StringBuilder();

      ComparisonOperatorsCodeGenerator.TryGet(
         ImplementedComparisonOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         "MyComparerAccessor",
         out var generator);

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("MyComparerAccessor.Comparer.Compare");
      result.Should().Contain("< 0");
      result.Should().Contain("<= 0");
      result.Should().Contain("> 0");
      result.Should().Contain(">= 0");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task UsingComparer_ValueType_GeneratesOperatorsWithoutNullChecks()
   {
      // Arrange
      var state = new ComparisonOperatorsStateBuilder()
                  .WithValueType("global::Thinktecture.Tests.MyStruct", "MyStruct")
                  .WithKeyMember("Value", "global::System.DateTime", isReferenceType: false)
                  .Build();

      var sb = new StringBuilder();

      ComparisonOperatorsCodeGenerator.TryGet(
         ImplementedComparisonOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         "ComparerAccessor",
         out var generator);

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("ComparerAccessor.Comparer.Compare");
      result.Should().NotContain("ArgumentNullException");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task UsingComparer_WithComplexComparerAccessor_GeneratesCorrectly()
   {
      // Arrange
      var state = new ComparisonOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.ProductId", "ProductId")
                  .WithKeyMember("_id", "global::System.Guid", isReferenceType: false)
                  .Build();

      var sb = new StringBuilder();

      ComparisonOperatorsCodeGenerator.TryGet(
         ImplementedComparisonOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         "global::Thinktecture.Tests.Comparers.ProductIdComparer",
         out var generator);

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("global::Thinktecture.Tests.Comparers.ProductIdComparer.Comparer.Compare");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task UsingOrdinalIgnoreCase_StringKey_ReferenceType_GeneratesStringComparerOrdinalIgnoreCase()
   {
      // Arrange
      var state = new ComparisonOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.ProductCode", "ProductCode")
                  .WithStringKeyMember("_code")
                  .Build();

      var sb = new StringBuilder();

      ComparisonOperatorsCodeGenerator.TryGet(
         ImplementedComparisonOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         null,
         out var generator);

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("global::System.StringComparer.OrdinalIgnoreCase.Compare");
      result.Should().Contain("< 0");
      result.Should().Contain("<= 0");
      result.Should().Contain("> 0");
      result.Should().Contain(">= 0");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task UsingOrdinalIgnoreCase_StringKey_ValueType_GeneratesStringComparerWithoutNullChecks()
   {
      // Arrange
      var state = new ComparisonOperatorsStateBuilder()
                  .WithValueType("global::Thinktecture.Tests.CodeStruct", "CodeStruct")
                  .WithStringKeyMember("Code")
                  .Build();

      var sb = new StringBuilder();

      ComparisonOperatorsCodeGenerator.TryGet(
         ImplementedComparisonOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         null,
         out var generator);

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("global::System.StringComparer.OrdinalIgnoreCase.Compare");
      result.Should().NotContain("ArgumentNullException");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task WithKeyTypeOverloads_UsingOperators_AllOperators_GeneratesBothDirections()
   {
      // Arrange
      var state = new ComparisonOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.Amount", "Amount")
                  .WithKeyMember("_value", "global::System.Decimal", isReferenceType: false)
                  .Build();

      var sb = new StringBuilder();

      ComparisonOperatorsCodeGenerator.TryGet(
         ImplementedComparisonOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.DefaultWithKeyTypeOverloads,
         null,
         out var generator);

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      // Should have type-to-type operators
      result.Should().Contain("operator <(global::Thinktecture.Tests.Amount left, global::Thinktecture.Tests.Amount right)");
      // Should have type-to-key overloads
      result.Should().Contain("operator <(global::Thinktecture.Tests.Amount left, global::System.Decimal right)");
      result.Should().Contain("operator <(global::System.Decimal left, global::Thinktecture.Tests.Amount right)");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task WithKeyTypeOverloads_UsingOperators_MixedNullability_TypeRefMemberValue_GeneratesCorrectNullChecks()
   {
      // Arrange
      var state = new ComparisonOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.Amount", "Amount")
                  .WithKeyMember("_value", "global::System.Int32", isReferenceType: false)
                  .Build();

      var sb = new StringBuilder();

      ComparisonOperatorsCodeGenerator.TryGet(
         ImplementedComparisonOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.DefaultWithKeyTypeOverloads,
         null,
         out var generator);

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      // Type-to-type should have null checks for left and right
      result.Should().Contain("operator <(global::Thinktecture.Tests.Amount left, global::Thinktecture.Tests.Amount right)");
      result.Should().Contain("global::System.ArgumentNullException.ThrowIfNull(left)");
      result.Should().Contain("global::System.ArgumentNullException.ThrowIfNull(right)");
      // Type-to-member should have null check for type
      result.Should().Contain("operator <(global::Thinktecture.Tests.Amount left, global::System.Int32 right)");
      // Member-to-type should have null check for type
      result.Should().Contain("operator <(global::System.Int32 left, global::Thinktecture.Tests.Amount right)");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task WithKeyTypeOverloads_UsingOperators_MixedNullability_TypeValueMemberRef_GeneratesCorrectNullChecks()
   {
      // Arrange
      var state = new ComparisonOperatorsStateBuilder()
                  .WithValueType("global::Thinktecture.Tests.ProductId", "ProductId")
                  .WithStringKeyMember("Value", isReferenceType: true)
                  .Build();

      var sb = new StringBuilder();

      ComparisonOperatorsCodeGenerator.TryGet(
         ImplementedComparisonOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.DefaultWithKeyTypeOverloads,
         null,
         out var generator);

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      // Type-to-type should have no null checks (both value types)
      result.Should().Contain("operator <(global::Thinktecture.Tests.ProductId left, global::Thinktecture.Tests.ProductId right)");
      // Type-to-member should have null check for string
      result.Should().Contain("operator <(global::Thinktecture.Tests.ProductId left, global::System.String right)");
      // Member-to-type should have null check for string
      result.Should().Contain("operator <(global::System.String left, global::Thinktecture.Tests.ProductId right)");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task WithKeyTypeOverloads_UsingComparer_GeneratesComparerOverloads()
   {
      // Arrange
      var state = new ComparisonOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.CustomType", "CustomType")
                  .WithKeyMember("_key", "global::System.Guid", isReferenceType: false)
                  .Build();

      var sb = new StringBuilder();

      ComparisonOperatorsCodeGenerator.TryGet(
         ImplementedComparisonOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.DefaultWithKeyTypeOverloads,
         "MyComparer",
         out var generator);

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("MyComparer.Comparer.Compare");
      result.Should().Contain("operator <(global::Thinktecture.Tests.CustomType left, global::Thinktecture.Tests.CustomType right)");
      result.Should().Contain("operator <(global::Thinktecture.Tests.CustomType left, global::System.Guid right)");
      result.Should().Contain("operator <(global::System.Guid left, global::Thinktecture.Tests.CustomType right)");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task WithKeyTypeOverloads_UsingOrdinalIgnoreCase_GeneratesStringComparerOverloads()
   {
      // Arrange
      var state = new ComparisonOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.ProductCode", "ProductCode")
                  .WithStringKeyMember("_code")
                  .Build();

      var sb = new StringBuilder();

      ComparisonOperatorsCodeGenerator.TryGet(
         ImplementedComparisonOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.DefaultWithKeyTypeOverloads,
         null,
         out var generator);

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("global::System.StringComparer.OrdinalIgnoreCase.Compare");
      result.Should().Contain("operator <(global::Thinktecture.Tests.ProductCode left, global::Thinktecture.Tests.ProductCode right)");
      result.Should().Contain("operator <(global::Thinktecture.Tests.ProductCode left, global::System.String right)");
      result.Should().Contain("operator <(global::System.String left, global::Thinktecture.Tests.ProductCode right)");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task WithKeyTypeOverloads_OnlyLessThan_GeneratesOnlyLessThanOverloads()
   {
      // Arrange
      var state = new ComparisonOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.Amount", "Amount")
                  .WithKeyMember("_value", "global::System.Decimal", isReferenceType: false)
                  .Build();

      var sb = new StringBuilder();

      ComparisonOperatorsCodeGenerator.TryGet(
         ImplementedComparisonOperators.LessThan,
         Thinktecture.CodeAnalysis.OperatorsGeneration.DefaultWithKeyTypeOverloads,
         null,
         out var generator);

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("operator <");
      result.Should().NotContain("operator <=");
      result.Should().NotContain("operator >");
      result.Should().NotContain("operator >=");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task WithKeyTypeOverloads_BothReferenceTypes_GeneratesAllNullChecks()
   {
      // Arrange
      var state = new ComparisonOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.ProductCode", "ProductCode")
                  .WithStringKeyMember("_code", isReferenceType: true)
                  .Build();

      var sb = new StringBuilder();

      ComparisonOperatorsCodeGenerator.TryGet(
         ImplementedComparisonOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.DefaultWithKeyTypeOverloads,
         null,
         out var generator);

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("global::System.ArgumentNullException.ThrowIfNull");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task WithDifferentKeyMemberName_UsesCorrectMemberName()
   {
      // Arrange
      var state = new ComparisonOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.CustomType", "CustomType")
                  .WithIntKeyMember("CustomKey")
                  .Build();

      var sb = new StringBuilder();

      ComparisonOperatorsCodeGenerator.TryGet(
         ImplementedComparisonOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         null,
         out var generator);

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("left.CustomKey");
      result.Should().Contain("right.CustomKey");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task WithComplexGenericTypeName_EscapesCorrectly()
   {
      // Arrange
      var state = new ComparisonOperatorsStateBuilder()
                  .WithType("global::Thinktecture.Tests.Generic<global::System.String, global::System.Int32>.Nested", "Nested")
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();

      ComparisonOperatorsCodeGenerator.TryGet(
         ImplementedComparisonOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         null,
         out var generator);

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task WithSpecialCharactersInTypeName_EscapesCorrectly()
   {
      // Arrange
      var state = new ComparisonOperatorsStateBuilder()
                  .WithType("global::Thinktecture.Tests.@Special.@Type", "Type")
                  .WithStringKeyMember()
                  .Build();

      var sb = new StringBuilder();

      ComparisonOperatorsCodeGenerator.TryGet(
         ImplementedComparisonOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         null,
         out var generator);

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }
}

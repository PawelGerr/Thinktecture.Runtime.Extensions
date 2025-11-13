using System.Text;
using System.Threading.Tasks;
using Thinktecture.CodeAnalysis;
using VerifyXunit;

namespace Thinktecture.Runtime.Tests.CodeAnalysis.ComparisonOperatorsCodeGeneratorTests;

public class GenerateBaseTypes
{
   [Fact]
   public void Should_not_generate_interface_when_no_operators_and_no_comparer_and_non_string_key()
   {
      // Arrange
      var state = new ComparisonOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.MyClass", "MyClass")
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();

      ComparisonOperatorsCodeGenerator.TryGet(
         ImplementedComparisonOperators.None,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         null,
         out var generator);

      // Act
      generator!.GenerateBaseTypes(sb, state);

      // Assert
      sb.Length.Should().Be(0, "should not generate any interfaces when no operators available");
   }

   [Fact]
   public async Task Should_generate_single_interface_without_key_type_overloads()
   {
      // Arrange
      var state = new ComparisonOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.TestType")
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();

      ComparisonOperatorsCodeGenerator.TryGet(
         ImplementedComparisonOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         null,
         out var generator);

      // Act
      generator!.GenerateBaseTypes(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task Should_generate_both_interfaces_with_key_type_overloads()
   {
      // Arrange
      var state = new ComparisonOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.TestType")
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();

      ComparisonOperatorsCodeGenerator.TryGet(
         ImplementedComparisonOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.DefaultWithKeyTypeOverloads,
         null,
         out var generator);

      // Act
      generator!.GenerateBaseTypes(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task Should_generate_interface_with_string_key_even_without_all_operators()
   {
      // Arrange
      var state = new ComparisonOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.ProductId", "ProductId")
                  .WithStringKeyMember()
                  .Build();

      var sb = new StringBuilder();

      ComparisonOperatorsCodeGenerator.TryGet(
         ImplementedComparisonOperators.LessThan,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         null,
         out var generator);

      // Act
      generator!.GenerateBaseTypes(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task Should_generate_interface_with_custom_comparer_even_without_all_operators()
   {
      // Arrange
      var state = new ComparisonOperatorsStateBuilder()
                  .WithValueType("global::Thinktecture.Tests.MyStruct", "MyStruct")
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();

      ComparisonOperatorsCodeGenerator.TryGet(
         ImplementedComparisonOperators.LessThan,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         "MyComparerAccessor",
         out var generator);

      // Act
      generator!.GenerateBaseTypes(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task Should_generate_interface_with_complex_generic_type_name()
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
      generator!.GenerateBaseTypes(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task Should_generate_interface_with_value_type()
   {
      // Arrange
      var state = new ComparisonOperatorsStateBuilder()
                  .WithValueType("global::Thinktecture.Tests.MyStruct", "MyStruct")
                  .WithGuidKeyMember()
                  .Build();

      var sb = new StringBuilder();

      ComparisonOperatorsCodeGenerator.TryGet(
         ImplementedComparisonOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         null,
         out var generator);

      // Act
      generator!.GenerateBaseTypes(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task Should_generate_interfaces_for_key_type_overloads_with_different_key_member_type()
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
      generator!.GenerateBaseTypes(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }
}

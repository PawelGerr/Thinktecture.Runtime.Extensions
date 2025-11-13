using System.Text;
using System.Threading.Tasks;
using Thinktecture.CodeAnalysis;
using Thinktecture.CodeAnalysis.ValueObjects;
using VerifyXunit;

namespace Thinktecture.Runtime.Tests.CodeAnalysis.DivisionOperatorsCodeGeneratorTests;

public class GenerateBaseTypes
{
   [Fact]
   public void Should_not_generate_interface_when_no_operators()
   {
      // Arrange
      var state = new DivisionOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.MyClass", "MyClass")
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();

      DivisionOperatorsCodeGenerator.TryGet(
         ImplementedOperators.None,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
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
      var state = new DivisionOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.TestType")
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();

      DivisionOperatorsCodeGenerator.TryGet(
         ImplementedOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
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
      var state = new DivisionOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.TestType")
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();

      DivisionOperatorsCodeGenerator.TryGet(
         ImplementedOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.DefaultWithKeyTypeOverloads,
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
      var state = new DivisionOperatorsStateBuilder()
                  .WithValueType("global::Thinktecture.Tests.Amount")
                  .WithDecimalKeyMember()
                  .Build();

      var sb = new StringBuilder();

      DivisionOperatorsCodeGenerator.TryGet(
         ImplementedOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
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
      var state = new DivisionOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.Generic<int, string>", "Generic<int, string>")
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();

      DivisionOperatorsCodeGenerator.TryGet(
         ImplementedOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
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
      var state = new DivisionOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.Counter", "Counter")
                  .WithByteKeyMember()
                  .Build();

      var sb = new StringBuilder();

      DivisionOperatorsCodeGenerator.TryGet(
         ImplementedOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.DefaultWithKeyTypeOverloads,
         out var generator);

      // Act
      generator!.GenerateBaseTypes(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public void Should_not_generate_interface_when_operators_do_not_include_all()
   {
      // Arrange
      var state = new DivisionOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.TestType")
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();

      DivisionOperatorsCodeGenerator.TryGet(
         ImplementedOperators.Default,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         out var generator);

      // Act
      generator!.GenerateBaseTypes(sb, state);

      // Assert
      sb.Length.Should().Be(0, "should not generate interfaces when operators don't include All");
   }

   [Fact]
   public void Should_not_generate_interface_when_checked_only()
   {
      // Arrange
      var state = new DivisionOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.TestType")
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();

      DivisionOperatorsCodeGenerator.TryGet(
         ImplementedOperators.Checked,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         out var generator);

      // Act
      generator!.GenerateBaseTypes(sb, state);

      // Assert
      sb.Length.Should().Be(0, "should not generate interfaces when only checked operators are specified");
   }
}

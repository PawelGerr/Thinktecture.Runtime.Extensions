using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Thinktecture.CodeAnalysis;
using Thinktecture.CodeAnalysis.ValueObjects;
using VerifyXunit;

namespace Thinktecture.Runtime.Tests.CodeAnalysis.SubtractionOperatorsCodeGeneratorTests;

public class GenerateImplementation
{
   [Fact]
   public async Task Should_generate_default_operator_only_for_reference_type()
   {
      // Arrange
      var state = new SubtractionOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.Amount", "Amount")
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();

      SubtractionOperatorsCodeGenerator.TryGet(
         ImplementedOperators.Default,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         out var generator);

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task Should_generate_checked_operator_only_for_reference_type()
   {
      // Arrange
      var state = new SubtractionOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.Amount", "Amount")
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();

      SubtractionOperatorsCodeGenerator.TryGet(
         ImplementedOperators.Checked,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         out var generator);

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task Should_generate_all_operators_for_reference_type()
   {
      // Arrange
      var state = new SubtractionOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.Amount", "Amount")
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();

      SubtractionOperatorsCodeGenerator.TryGet(
         ImplementedOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         out var generator);

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task Should_generate_all_operators_for_value_type()
   {
      // Arrange
      var state = new SubtractionOperatorsStateBuilder()
                  .WithValueType("global::Thinktecture.Tests.Amount", "Amount")
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();

      SubtractionOperatorsCodeGenerator.TryGet(
         ImplementedOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         out var generator);

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task Should_generate_with_cast_when_arithmetic_operation_yields_different_type()
   {
      // Arrange - byte - byte = int, so needs cast back to byte
      var state = new SubtractionOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.Counter", "Counter")
                  .WithByteKeyMember()
                  .Build();

      var sb = new StringBuilder();

      SubtractionOperatorsCodeGenerator.TryGet(
         ImplementedOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         out var generator);

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task Should_generate_without_cast_when_arithmetic_operation_yields_same_type()
   {
      // Arrange - decimal - decimal = decimal, no cast needed
      var state = new SubtractionOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.Price", "Price")
                  .WithDecimalKeyMember()
                  .Build();

      var sb = new StringBuilder();

      SubtractionOperatorsCodeGenerator.TryGet(
         ImplementedOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         out var generator);

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task Should_generate_with_different_key_member_name()
   {
      // Arrange
      var state = new SubtractionOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.Amount", "Amount")
                  .WithIntKeyMember("_amount")
                  .Build();

      var sb = new StringBuilder();

      SubtractionOperatorsCodeGenerator.TryGet(
         ImplementedOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         out var generator);

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task Should_generate_with_different_create_factory_method_name()
   {
      // Arrange
      var state = new SubtractionOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.Amount", "Amount")
                  .WithIntKeyMember()
                  .WithCreateFactoryMethodName("CreateAmount")
                  .Build();

      var sb = new StringBuilder();

      SubtractionOperatorsCodeGenerator.TryGet(
         ImplementedOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         out var generator);

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task Should_generate_with_complex_generic_type_name()
   {
      // Arrange
      var state = new SubtractionOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.GenericValue<int, string>", "GenericValue<int, string>")
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();

      SubtractionOperatorsCodeGenerator.TryGet(
         ImplementedOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         out var generator);

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task Should_generate_key_type_overloads_for_reference_type_with_value_type_key()
   {
      // Arrange
      var state = new SubtractionOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.Amount", "Amount")
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();

      SubtractionOperatorsCodeGenerator.TryGet(
         ImplementedOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.DefaultWithKeyTypeOverloads,
         out var generator);

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task Should_generate_key_type_overloads_for_value_type_with_value_type_key()
   {
      // Arrange
      var state = new SubtractionOperatorsStateBuilder()
                  .WithValueType("global::Thinktecture.Tests.Amount", "Amount")
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();

      SubtractionOperatorsCodeGenerator.TryGet(
         ImplementedOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.DefaultWithKeyTypeOverloads,
         out var generator);

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task Should_generate_key_type_overloads_for_reference_type_with_reference_type_key()
   {
      // Arrange
      var state = new SubtractionOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.Text", "Text")
                  .WithStringKeyMember()
                  .Build();

      var sb = new StringBuilder();

      SubtractionOperatorsCodeGenerator.TryGet(
         ImplementedOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.DefaultWithKeyTypeOverloads,
         out var generator);

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task Should_generate_key_type_overloads_for_value_type_with_reference_type_key()
   {
      // Arrange
      var state = new SubtractionOperatorsStateBuilder()
                  .WithValueType("global::Thinktecture.Tests.Text", "Text")
                  .WithStringKeyMember()
                  .Build();

      var sb = new StringBuilder();

      SubtractionOperatorsCodeGenerator.TryGet(
         ImplementedOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.DefaultWithKeyTypeOverloads,
         out var generator);

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task Should_generate_key_type_overloads_with_default_operator_only()
   {
      // Arrange
      var state = new SubtractionOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.Amount", "Amount")
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();

      SubtractionOperatorsCodeGenerator.TryGet(
         ImplementedOperators.Default,
         Thinktecture.CodeAnalysis.OperatorsGeneration.DefaultWithKeyTypeOverloads,
         out var generator);

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task Should_generate_key_type_overloads_with_checked_operator_only()
   {
      // Arrange
      var state = new SubtractionOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.Amount", "Amount")
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();

      SubtractionOperatorsCodeGenerator.TryGet(
         ImplementedOperators.Checked,
         Thinktecture.CodeAnalysis.OperatorsGeneration.DefaultWithKeyTypeOverloads,
         out var generator);

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task Should_generate_key_type_overloads_with_cast_for_byte_key()
   {
      // Arrange
      var state = new SubtractionOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.Counter", "Counter")
                  .WithByteKeyMember()
                  .Build();

      var sb = new StringBuilder();

      SubtractionOperatorsCodeGenerator.TryGet(
         ImplementedOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.DefaultWithKeyTypeOverloads,
         out var generator);

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task Should_generate_with_char_key_member_requiring_cast()
   {
      // Arrange - char - char = int, needs cast
      var state = new SubtractionOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.CharValue", "CharValue")
                  .WithKeyMember("Value", "global::System.Char", SpecialType.System_Char, isReferenceType: false)
                  .Build();

      var sb = new StringBuilder();

      SubtractionOperatorsCodeGenerator.TryGet(
         ImplementedOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         out var generator);

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task Should_generate_with_short_key_member_requiring_cast()
   {
      // Arrange - short - short = int, needs cast
      var state = new SubtractionOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.ShortValue", "ShortValue")
                  .WithKeyMember("Value", "global::System.Int16", SpecialType.System_Int16, isReferenceType: false)
                  .Build();

      var sb = new StringBuilder();

      SubtractionOperatorsCodeGenerator.TryGet(
         ImplementedOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         out var generator);

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task Should_generate_with_sbyte_key_member_requiring_cast()
   {
      // Arrange - sbyte - sbyte = int, needs cast
      var state = new SubtractionOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.SByteValue", "SByteValue")
                  .WithKeyMember("Value", "global::System.SByte", SpecialType.System_SByte, isReferenceType: false)
                  .Build();

      var sb = new StringBuilder();

      SubtractionOperatorsCodeGenerator.TryGet(
         ImplementedOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         out var generator);

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task Should_generate_with_ushort_key_member_requiring_cast()
   {
      // Arrange - ushort - ushort = int, needs cast
      var state = new SubtractionOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.UShortValue", "UShortValue")
                  .WithKeyMember("Value", "global::System.UInt16", SpecialType.System_UInt16, isReferenceType: false)
                  .Build();

      var sb = new StringBuilder();

      SubtractionOperatorsCodeGenerator.TryGet(
         ImplementedOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         out var generator);

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public void Should_not_generate_operators_when_none_specified()
   {
      // Arrange
      var state = new SubtractionOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.Amount", "Amount")
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();

      SubtractionOperatorsCodeGenerator.TryGet(
         ImplementedOperators.None,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         out var generator);

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      sb.Length.Should().Be(0, "should not generate operators when None is specified");
   }

   [Fact]
   public async Task Should_handle_nullable_reference_type_properly()
   {
      // Arrange - testing edge case with nullable string key
      var state = new SubtractionOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.NullableText", "NullableText")
                  .WithStringKeyMember(isReferenceType: true)
                  .Build();

      var sb = new StringBuilder();

      SubtractionOperatorsCodeGenerator.TryGet(
         ImplementedOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         out var generator);

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }
}

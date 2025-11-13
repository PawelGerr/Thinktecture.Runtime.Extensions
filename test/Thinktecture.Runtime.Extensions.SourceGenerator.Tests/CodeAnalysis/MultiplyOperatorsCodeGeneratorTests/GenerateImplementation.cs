using System.Text;
using System.Threading.Tasks;
using Thinktecture.CodeAnalysis;
using Thinktecture.CodeAnalysis.ValueObjects;
using VerifyXunit;

namespace Thinktecture.Runtime.Tests.CodeAnalysis.MultiplyOperatorsCodeGeneratorTests;

public class GenerateImplementation
{
   [Fact]
   public async Task Should_generate_default_operator_only_for_reference_type()
   {
      // Arrange
      var state = new MultiplyOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.Amount", "Amount")
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();

      MultiplyOperatorsCodeGenerator.TryGet(
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
      var state = new MultiplyOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.Amount", "Amount")
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();

      MultiplyOperatorsCodeGenerator.TryGet(
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
      var state = new MultiplyOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.Amount", "Amount")
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();

      MultiplyOperatorsCodeGenerator.TryGet(
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
      var state = new MultiplyOperatorsStateBuilder()
                  .WithValueType("global::Thinktecture.Tests.Amount", "Amount")
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();

      MultiplyOperatorsCodeGenerator.TryGet(
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
      // Arrange - byte * byte = int, so needs cast back to byte
      var state = new MultiplyOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.Counter", "Counter")
                  .WithByteKeyMember()
                  .Build();

      var sb = new StringBuilder();

      MultiplyOperatorsCodeGenerator.TryGet(
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
      // Arrange - decimal * decimal = decimal, no cast needed
      var state = new MultiplyOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.Price", "Price")
                  .WithDecimalKeyMember()
                  .Build();

      var sb = new StringBuilder();

      MultiplyOperatorsCodeGenerator.TryGet(
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
      var state = new MultiplyOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.Amount", "Amount")
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();

      MultiplyOperatorsCodeGenerator.TryGet(
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
      var state = new MultiplyOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.Name", "Name")
                  .WithStringKeyMember()
                  .Build();

      var sb = new StringBuilder();

      MultiplyOperatorsCodeGenerator.TryGet(
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
      var state = new MultiplyOperatorsStateBuilder()
                  .WithValueType("global::Thinktecture.Tests.Amount", "Amount")
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();

      MultiplyOperatorsCodeGenerator.TryGet(
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
      var state = new MultiplyOperatorsStateBuilder()
                  .WithValueType("global::Thinktecture.Tests.Name", "Name")
                  .WithStringKeyMember()
                  .Build();

      var sb = new StringBuilder();

      MultiplyOperatorsCodeGenerator.TryGet(
         ImplementedOperators.All,
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
      // Arrange - byte * byte = int, needs cast
      var state = new MultiplyOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.Counter", "Counter")
                  .WithByteKeyMember()
                  .Build();

      var sb = new StringBuilder();

      MultiplyOperatorsCodeGenerator.TryGet(
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
      var state = new MultiplyOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.Amount", "Amount")
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();

      MultiplyOperatorsCodeGenerator.TryGet(
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
      var state = new MultiplyOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.Amount", "Amount")
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();

      MultiplyOperatorsCodeGenerator.TryGet(
         ImplementedOperators.Checked,
         Thinktecture.CodeAnalysis.OperatorsGeneration.DefaultWithKeyTypeOverloads,
         out var generator);

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task Should_generate_with_sbyte_key_member_requiring_cast()
   {
      // Arrange - sbyte * sbyte = int, needs cast
      var state = new MultiplyOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.SignedCounter", "SignedCounter")
                  .WithSByteKeyMember()
                  .Build();

      var sb = new StringBuilder();

      MultiplyOperatorsCodeGenerator.TryGet(
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
      // Arrange - short * short = int, needs cast
      var state = new MultiplyOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.SmallNumber", "SmallNumber")
                  .WithShortKeyMember()
                  .Build();

      var sb = new StringBuilder();

      MultiplyOperatorsCodeGenerator.TryGet(
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
      // Arrange - ushort * ushort = int, needs cast
      var state = new MultiplyOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.UnsignedSmallNumber", "UnsignedSmallNumber")
                  .WithUShortKeyMember()
                  .Build();

      var sb = new StringBuilder();

      MultiplyOperatorsCodeGenerator.TryGet(
         ImplementedOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         out var generator);

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task Should_generate_with_char_key_member_requiring_cast()
   {
      // Arrange - char * char = int, needs cast
      var state = new MultiplyOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.CharValue", "CharValue")
                  .WithCharKeyMember()
                  .Build();

      var sb = new StringBuilder();

      MultiplyOperatorsCodeGenerator.TryGet(
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
      var state = new MultiplyOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.Amount", "Amount")
                  .WithIntKeyMember()
                  .WithCreateFactoryMethodName("CreateNew")
                  .Build();

      var sb = new StringBuilder();

      MultiplyOperatorsCodeGenerator.TryGet(
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
      var state = new MultiplyOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.Amount", "Amount")
                  .WithIntKeyMember("_amount")
                  .Build();

      var sb = new StringBuilder();

      MultiplyOperatorsCodeGenerator.TryGet(
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
      var state = new MultiplyOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.Generic<int, string>", "Generic<int, string>")
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();

      MultiplyOperatorsCodeGenerator.TryGet(
         ImplementedOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         out var generator);

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }
}

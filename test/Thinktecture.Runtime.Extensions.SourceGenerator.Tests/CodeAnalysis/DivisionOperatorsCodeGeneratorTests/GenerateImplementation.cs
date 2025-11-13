using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Thinktecture.CodeAnalysis;
using Thinktecture.CodeAnalysis.ValueObjects;
using VerifyXunit;

namespace Thinktecture.Runtime.Tests.CodeAnalysis.DivisionOperatorsCodeGeneratorTests;

public class GenerateImplementation
{
   [Fact]
   public async Task Should_generate_default_operator_only_for_reference_type()
   {
      // Arrange
      var state = new DivisionOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.Amount", "Amount")
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();

      DivisionOperatorsCodeGenerator.TryGet(
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
      var state = new DivisionOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.Amount", "Amount")
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();

      DivisionOperatorsCodeGenerator.TryGet(
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
      var state = new DivisionOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.Amount", "Amount")
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();

      DivisionOperatorsCodeGenerator.TryGet(
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
      var state = new DivisionOperatorsStateBuilder()
                  .WithValueType("global::Thinktecture.Tests.Amount", "Amount")
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();

      DivisionOperatorsCodeGenerator.TryGet(
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
      // Arrange - byte / byte = int, so needs cast back to byte
      var state = new DivisionOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.Counter", "Counter")
                  .WithByteKeyMember()
                  .Build();

      var sb = new StringBuilder();

      DivisionOperatorsCodeGenerator.TryGet(
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
      // Arrange - decimal / decimal = decimal, no cast needed
      var state = new DivisionOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.Price", "Price")
                  .WithDecimalKeyMember()
                  .Build();

      var sb = new StringBuilder();

      DivisionOperatorsCodeGenerator.TryGet(
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
      var state = new DivisionOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.Amount", "Amount")
                  .WithIntKeyMember("_amount")
                  .Build();

      var sb = new StringBuilder();

      DivisionOperatorsCodeGenerator.TryGet(
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
      var state = new DivisionOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.Amount", "Amount")
                  .WithIntKeyMember()
                  .WithCreateFactoryMethodName("CreateAmount")
                  .Build();

      var sb = new StringBuilder();

      DivisionOperatorsCodeGenerator.TryGet(
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
      var state = new DivisionOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.GenericValue<int, string>", "GenericValue<int, string>")
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();

      DivisionOperatorsCodeGenerator.TryGet(
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
      var state = new DivisionOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.Amount", "Amount")
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();

      DivisionOperatorsCodeGenerator.TryGet(
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
      var state = new DivisionOperatorsStateBuilder()
                  .WithValueType("global::Thinktecture.Tests.Amount", "Amount")
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();

      DivisionOperatorsCodeGenerator.TryGet(
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
      var state = new DivisionOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.Text", "Text")
                  .WithStringKeyMember()
                  .Build();

      var sb = new StringBuilder();

      DivisionOperatorsCodeGenerator.TryGet(
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
      var state = new DivisionOperatorsStateBuilder()
                  .WithValueType("global::Thinktecture.Tests.Text", "Text")
                  .WithStringKeyMember()
                  .Build();

      var sb = new StringBuilder();

      DivisionOperatorsCodeGenerator.TryGet(
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
      var state = new DivisionOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.Amount", "Amount")
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();

      DivisionOperatorsCodeGenerator.TryGet(
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
      var state = new DivisionOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.Amount", "Amount")
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();

      DivisionOperatorsCodeGenerator.TryGet(
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
      generator!.GenerateImplementation(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task Should_generate_with_char_key_member_requiring_cast()
   {
      // Arrange - char / char = int, needs cast
      var state = new DivisionOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.CharValue", "CharValue")
                  .WithKeyMember("Value", "global::System.Char", SpecialType.System_Char, isReferenceType: false)
                  .Build();

      var sb = new StringBuilder();

      DivisionOperatorsCodeGenerator.TryGet(
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
      // Arrange - short / short = int, needs cast
      var state = new DivisionOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.ShortValue", "ShortValue")
                  .WithKeyMember("Value", "global::System.Int16", SpecialType.System_Int16, isReferenceType: false)
                  .Build();

      var sb = new StringBuilder();

      DivisionOperatorsCodeGenerator.TryGet(
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
      // Arrange - sbyte / sbyte = int, needs cast
      var state = new DivisionOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.SByteValue", "SByteValue")
                  .WithKeyMember("Value", "global::System.SByte", SpecialType.System_SByte, isReferenceType: false)
                  .Build();

      var sb = new StringBuilder();

      DivisionOperatorsCodeGenerator.TryGet(
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
      // Arrange - ushort / ushort = int, needs cast
      var state = new DivisionOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.UShortValue", "UShortValue")
                  .WithKeyMember("Value", "global::System.UInt16", SpecialType.System_UInt16, isReferenceType: false)
                  .Build();

      var sb = new StringBuilder();

      DivisionOperatorsCodeGenerator.TryGet(
         ImplementedOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         out var generator);

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task Should_handle_nullable_reference_type_properly()
   {
      // Arrange - nullable reference type should generate null checks
      var state = new DivisionOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.Amount", "Amount")
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();

      DivisionOperatorsCodeGenerator.TryGet(
         ImplementedOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         out var generator);

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert - verify null checks are present
      var output = sb.ToString();
      output.Should().Contain("ThrowIfNull(left)", "should generate null check for left operand");
      output.Should().Contain("ThrowIfNull(right)", "should generate null check for right operand");
      await Verifier.Verify(output);
   }

   [Fact]
   public void Should_not_generate_null_checks_for_value_types()
   {
      // Arrange
      var state = new DivisionOperatorsStateBuilder()
                  .WithValueType("global::Thinktecture.Tests.Amount", "Amount")
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();

      DivisionOperatorsCodeGenerator.TryGet(
         ImplementedOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         out var generator);

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      var output = sb.ToString();
      output.Should().NotContain("ThrowIfNull", "should not generate null checks for value types");
   }

   [Fact]
   public async Task Should_generate_null_check_for_string_key_member_in_overloads()
   {
      // Arrange
      var state = new DivisionOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.Text", "Text")
                  .WithStringKeyMember()
                  .Build();

      var sb = new StringBuilder();

      DivisionOperatorsCodeGenerator.TryGet(
         ImplementedOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.DefaultWithKeyTypeOverloads,
         out var generator);

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert - verify null checks for string key member
      var output = sb.ToString();
      output.Should().Contain("ThrowIfNull", "should generate null checks for reference type key member");
      await Verifier.Verify(output);
   }

   [Fact]
   public async Task Should_generate_with_uint_key_member()
   {
      // Arrange - uint / uint = uint, no cast needed
      var state = new DivisionOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.UIntValue", "UIntValue")
                  .WithKeyMember("Value", "global::System.UInt32", SpecialType.System_UInt32, isReferenceType: false)
                  .Build();

      var sb = new StringBuilder();

      DivisionOperatorsCodeGenerator.TryGet(
         ImplementedOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         out var generator);

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task Should_generate_with_long_key_member()
   {
      // Arrange - long / long = long, no cast needed
      var state = new DivisionOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.LongValue", "LongValue")
                  .WithKeyMember("Value", "global::System.Int64", SpecialType.System_Int64, isReferenceType: false)
                  .Build();

      var sb = new StringBuilder();

      DivisionOperatorsCodeGenerator.TryGet(
         ImplementedOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         out var generator);

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task Should_generate_with_ulong_key_member()
   {
      // Arrange - ulong / ulong = ulong, no cast needed
      var state = new DivisionOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.ULongValue", "ULongValue")
                  .WithKeyMember("Value", "global::System.UInt64", SpecialType.System_UInt64, isReferenceType: false)
                  .Build();

      var sb = new StringBuilder();

      DivisionOperatorsCodeGenerator.TryGet(
         ImplementedOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         out var generator);

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task Should_generate_with_float_key_member()
   {
      // Arrange - float / float = float, no cast needed
      var state = new DivisionOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.FloatValue", "FloatValue")
                  .WithKeyMember("Value", "global::System.Single", SpecialType.System_Single, isReferenceType: false)
                  .Build();

      var sb = new StringBuilder();

      DivisionOperatorsCodeGenerator.TryGet(
         ImplementedOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         out var generator);

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task Should_generate_with_double_key_member()
   {
      // Arrange - double / double = double, no cast needed
      var state = new DivisionOperatorsStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.DoubleValue", "DoubleValue")
                  .WithKeyMember("Value", "global::System.Double", SpecialType.System_Double, isReferenceType: false)
                  .Build();

      var sb = new StringBuilder();

      DivisionOperatorsCodeGenerator.TryGet(
         ImplementedOperators.All,
         Thinktecture.CodeAnalysis.OperatorsGeneration.Default,
         out var generator);

      // Act
      generator!.GenerateImplementation(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }
}

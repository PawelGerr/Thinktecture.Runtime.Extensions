using System.Text;
using System.Threading.Tasks;
using Thinktecture.CodeAnalysis;
using VerifyXunit;

namespace Thinktecture.Runtime.Tests.CodeAnalysis.FormattableCodeGeneratorTests;

public class GenerateBaseTypes
{
   [Fact]
   public void GenerateBaseTypes_WithEmptyStringBuilder_AppendsContent()
   {
      // Arrange
      var state = new FormattableStateBuilder()
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();
      var generator = FormattableCodeGenerator.Instance;

      // Act
      generator.GenerateBaseTypes(sb, state);

      // Assert
      sb.Length.Should().BeGreaterThan(0);
   }

   [Fact]
   public async Task GenerateBaseTypes_WithSimpleType_GeneratesCorrectInterfaces()
   {
      // Arrange
      var state = new FormattableStateBuilder()
                  .WithType("global::Thinktecture.Tests.TestType")
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();
      var generator = FormattableCodeGenerator.Instance;

      // Act
      generator.GenerateBaseTypes(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task GenerateBaseTypes_WithReferenceType_GeneratesCorrectly()
   {
      // Arrange
      var state = new FormattableStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.MyClass", "MyClass")
                  .WithStringKeyMember()
                  .Build();

      var sb = new StringBuilder();
      var generator = FormattableCodeGenerator.Instance;

      // Act
      generator.GenerateBaseTypes(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task GenerateBaseTypes_WithValueType_GeneratesCorrectly()
   {
      // Arrange
      var state = new FormattableStateBuilder()
                  .WithValueType("global::Thinktecture.Tests.MyStruct", "MyStruct")
                  .WithGuidKeyMember()
                  .Build();

      var sb = new StringBuilder();
      var generator = FormattableCodeGenerator.Instance;

      // Act
      generator.GenerateBaseTypes(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task GenerateBaseTypes_WithComplexGenericTypeName_EscapesCorrectly()
   {
      // Arrange
      var state = new FormattableStateBuilder()
                  .WithType("global::Thinktecture.Tests.Generic<global::System.String, global::System.Int32>.Nested", "Nested")
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();
      var generator = FormattableCodeGenerator.Instance;

      // Act
      generator.GenerateBaseTypes(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task GenerateBaseTypes_WithSpecialCharactersInTypeName_EscapesCorrectly()
   {
      // Arrange
      var state = new FormattableStateBuilder()
                  .WithType("global::Thinktecture.Tests.@Special.@Type", "Type")
                  .WithStringKeyMember()
                  .Build();

      var sb = new StringBuilder();
      var generator = FormattableCodeGenerator.Instance;

      // Act
      generator.GenerateBaseTypes(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task GenerateBaseTypes_MultipleCallsToSameStringBuilder_AppendsContent()
   {
      // Arrange
      var state1 = new FormattableStateBuilder()
                   .WithType("global::Thinktecture.Tests.Type1", "Type1")
                   .WithIntKeyMember()
                   .Build();

      var state2 = new FormattableStateBuilder()
                   .WithType("global::Thinktecture.Tests.Type2", "Type2")
                   .WithStringKeyMember()
                   .Build();

      var sb = new StringBuilder();
      var generator = FormattableCodeGenerator.Instance;

      // Act
      generator.GenerateBaseTypes(sb, state1);
      generator.GenerateBaseTypes(sb, state2);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("IFormattable");
      await Verifier.Verify(result);
   }
}

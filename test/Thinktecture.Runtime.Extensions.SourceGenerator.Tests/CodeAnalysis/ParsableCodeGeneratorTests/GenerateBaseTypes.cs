using System.Text;
using System.Threading.Tasks;
using Thinktecture.CodeAnalysis;
using VerifyXunit;

namespace Thinktecture.Runtime.Tests.CodeAnalysis.ParsableCodeGeneratorTests;

public class GenerateBaseTypes
{
   [Fact]
   public void GenerateBaseTypes_WithEmptyStringBuilder_AppendsContent()
   {
      // Arrange
      var state = new ParsableStateBuilder()
                  .WithStringKeyMember()
                  .WithIsEnum()
                  .Build();

      var sb = new StringBuilder();

      // Act
      ParsableCodeGenerator.ForEnum.GenerateBaseTypes(sb, state);

      // Assert
      sb.Length.Should().BeGreaterThan(0);
   }

   [Fact]
   public async Task GenerateBaseTypes_MultipleCallsToSameStringBuilder_AppendsContent()
   {
      // Arrange
      var state1 = new ParsableStateBuilder()
                   .WithType("global::Thinktecture.Tests.Type1", "Type1")
                   .WithStringKeyMember()
                   .WithIsEnum()
                   .Build();

      var state2 = new ParsableStateBuilder()
                   .WithType("global::Thinktecture.Tests.Type2", "Type2")
                   .WithIntKeyMember()
                   .Build();

      var sb = new StringBuilder();

      // Act
      ParsableCodeGenerator.ForEnum.GenerateBaseTypes(sb, state1);
      ParsableCodeGenerator.ForValueObject.GenerateBaseTypes(sb, state2);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("Type1");
      result.Should().Contain("Type2");
      result.Should().Contain("ISpanParsable");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task ForValueObject_Always_GeneratesIParsableOnly()
   {
      // Arrange
      var state = new ParsableStateBuilder()
                  .WithType("global::Thinktecture.Tests.TestValueObject", "TestValueObject")
                  .WithStringKeyMember()
                  .Build();

      var sb = new StringBuilder();

      // Act
      ParsableCodeGenerator.ForValueObject.GenerateBaseTypes(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task ForValueObject_WithNonStringKey_GeneratesIParsableOnly()
   {
      // Arrange
      var state = new ParsableStateBuilder()
                  .WithType("global::Thinktecture.Tests.TestValueObject", "TestValueObject")
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();

      // Act
      ParsableCodeGenerator.ForValueObject.GenerateBaseTypes(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task ForValueObject_WithoutKey_GeneratesIParsableOnly()
   {
      // Arrange
      var state = new ParsableStateBuilder()
                  .WithType("global::Thinktecture.Tests.TestValueObject", "TestValueObject")
                  .WithoutKeyMember()
                  .Build();

      var sb = new StringBuilder();

      // Act
      ParsableCodeGenerator.ForValueObject.GenerateBaseTypes(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task ForEnum_WithStringKey_GeneratesIParsableAndISpanParsable()
   {
      // Arrange
      var state = new ParsableStateBuilder()
                  .WithType("global::Thinktecture.Tests.TestEnum", "TestEnum")
                  .WithStringKeyMember()
                  .WithIsEnum()
                  .Build();

      var sb = new StringBuilder();

      // Act
      ParsableCodeGenerator.ForEnum.GenerateBaseTypes(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task ForEnum_WithIntKey_GeneratesIParsableOnly()
   {
      // Arrange
      var state = new ParsableStateBuilder()
                  .WithType("global::Thinktecture.Tests.TestEnum", "TestEnum")
                  .WithIntKeyMember()
                  .WithIsEnum()
                  .Build();

      var sb = new StringBuilder();

      // Act
      ParsableCodeGenerator.ForEnum.GenerateBaseTypes(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task ForEnum_WithGuidKey_GeneratesIParsableOnly()
   {
      // Arrange
      var state = new ParsableStateBuilder()
                  .WithType("global::Thinktecture.Tests.TestEnum", "TestEnum")
                  .WithGuidKeyMember()
                  .WithIsEnum()
                  .Build();

      var sb = new StringBuilder();

      // Act
      ParsableCodeGenerator.ForEnum.GenerateBaseTypes(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task ForEnum_WithoutKey_GeneratesIParsableOnly()
   {
      // Arrange
      var state = new ParsableStateBuilder()
                  .WithType("global::Thinktecture.Tests.TestEnum", "TestEnum")
                  .WithoutKeyMember()
                  .WithIsEnum()
                  .Build();

      var sb = new StringBuilder();

      // Act
      ParsableCodeGenerator.ForEnum.GenerateBaseTypes(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task ForEnum_WithComplexTypeName_EscapesCorrectly()
   {
      // Arrange
      var state = new ParsableStateBuilder()
                  .WithType("global::Thinktecture.Tests.Nested.Complex<T>.InnerEnum", "InnerEnum")
                  .WithStringKeyMember()
                  .WithIsEnum()
                  .Build();

      var sb = new StringBuilder();

      // Act
      ParsableCodeGenerator.ForEnum.GenerateBaseTypes(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }
}

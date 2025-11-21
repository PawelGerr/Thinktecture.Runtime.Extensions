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
                  .Build();

      var sb = new StringBuilder();

      // Act
      ParsableCodeGenerator.Instance.GenerateBaseTypes(sb, state);

      // Assert
      sb.Length.Should().BeGreaterThan(0);
   }

   [Fact]
   public async Task GenerateBaseTypes_MultipleCallsToSameStringBuilder_AppendsContent()
   {
      // Arrange
      var state = new ParsableStateBuilder()
                  .WithType("global::Thinktecture.Tests.Type1", "Type1")
                  .WithStringKeyMember()
                  .Build();

      var sb = new StringBuilder();

      // Act
      ParsableCodeGenerator.Instance.GenerateBaseTypes(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("Type1");
      result.Should().Contain("IParsable");
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
      ParsableCodeGenerator.Instance.GenerateBaseTypes(sb, state);

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
      ParsableCodeGenerator.Instance.GenerateBaseTypes(sb, state);

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
      ParsableCodeGenerator.Instance.GenerateBaseTypes(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task ForEnum_WithStringKey_GeneratesIParsableOnly()
   {
      // Arrange
      var state = new ParsableStateBuilder()
                  .WithType("global::Thinktecture.Tests.TestEnum", "TestEnum")
                  .WithStringKeyMember()
                  .Build();

      var sb = new StringBuilder();

      // Act
      ParsableCodeGenerator.Instance.GenerateBaseTypes(sb, state);

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
                  .Build();

      var sb = new StringBuilder();

      // Act
      ParsableCodeGenerator.Instance.GenerateBaseTypes(sb, state);

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
                  .Build();

      var sb = new StringBuilder();

      // Act
      ParsableCodeGenerator.Instance.GenerateBaseTypes(sb, state);

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
                  .Build();

      var sb = new StringBuilder();

      // Act
      ParsableCodeGenerator.Instance.GenerateBaseTypes(sb, state);

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
                  .Build();

      var sb = new StringBuilder();

      // Act
      ParsableCodeGenerator.Instance.GenerateBaseTypes(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }
}

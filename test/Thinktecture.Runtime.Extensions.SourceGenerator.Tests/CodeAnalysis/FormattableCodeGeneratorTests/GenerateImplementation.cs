using System.Text;
using System.Threading.Tasks;
using Thinktecture.CodeAnalysis;
using VerifyXunit;

namespace Thinktecture.Runtime.Tests.CodeAnalysis.FormattableCodeGeneratorTests;

public class GenerateImplementation
{
   [Fact]
   public void GenerateImplementation_WithEmptyStringBuilder_AppendsContent()
   {
      // Arrange
      var state = new FormattableStateBuilder()
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();
      var generator = FormattableCodeGenerator.Instance;

      // Act
      generator.GenerateImplementation(sb, state);

      // Assert
      sb.Length.Should().BeGreaterThan(0);
   }

   [Fact]
   public async Task WithStringKeyMember_GeneratesCorrectToStringMethod()
   {
      // Arrange
      var state = new FormattableStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.ProductId", "ProductId")
                  .WithStringKeyMember("_value")
                  .Build();

      var sb = new StringBuilder();
      var generator = FormattableCodeGenerator.Instance;

      // Act
      generator.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("public string ToString(string? format, global::System.IFormatProvider? formatProvider = null)");
      result.Should().Contain("return ((global::System.IFormattable)this._value).ToString(format, formatProvider);");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task WithIntKeyMember_GeneratesCorrectToStringMethod()
   {
      // Arrange
      var state = new FormattableStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.Amount", "Amount")
                  .WithIntKeyMember("_value")
                  .Build();

      var sb = new StringBuilder();
      var generator = FormattableCodeGenerator.Instance;

      // Act
      generator.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("public string ToString(string? format, global::System.IFormatProvider? formatProvider = null)");
      result.Should().Contain("return ((global::System.IFormattable)this._value).ToString(format, formatProvider);");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task WithGuidKeyMember_GeneratesCorrectToStringMethod()
   {
      // Arrange
      var state = new FormattableStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.OrderId", "OrderId")
                  .WithGuidKeyMember("_id")
                  .Build();

      var sb = new StringBuilder();
      var generator = FormattableCodeGenerator.Instance;

      // Act
      generator.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("return ((global::System.IFormattable)this._id).ToString(format, formatProvider);");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task WithDecimalKeyMember_GeneratesCorrectToStringMethod()
   {
      // Arrange
      var state = new FormattableStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.Price", "Price")
                  .WithDecimalKeyMember("Value")
                  .Build();

      var sb = new StringBuilder();
      var generator = FormattableCodeGenerator.Instance;

      // Act
      generator.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("return ((global::System.IFormattable)this.Value).ToString(format, formatProvider);");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task WithDifferentKeyMemberNames_GeneratesCorrectly()
   {
      // Arrange
      var state = new FormattableStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.TestType")
                  .WithIntKeyMember("CustomKeyName")
                  .Build();

      var sb = new StringBuilder();
      var generator = FormattableCodeGenerator.Instance;

      // Act
      generator.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("this.CustomKeyName").And.Contain("ToString(format, formatProvider)");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task WithValueType_GeneratesCorrectly()
   {
      // Arrange
      var state = new FormattableStateBuilder()
                  .WithValueType("global::Thinktecture.Tests.MyStruct", "MyStruct")
                  .WithIntKeyMember("_value")
                  .Build();

      var sb = new StringBuilder();
      var generator = FormattableCodeGenerator.Instance;

      // Act
      generator.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("public string ToString(string? format, global::System.IFormatProvider? formatProvider = null)");
      result.Should().Contain("return ((global::System.IFormattable)this._value).ToString(format, formatProvider);");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task WithReferenceType_GeneratesCorrectly()
   {
      // Arrange
      var state = new FormattableStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.MyClass", "MyClass")
                  .WithStringKeyMember("Key")
                  .Build();

      var sb = new StringBuilder();
      var generator = FormattableCodeGenerator.Instance;

      // Act
      generator.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("public string ToString(string? format, global::System.IFormatProvider? formatProvider = null)");
      result.Should().Contain("return ((global::System.IFormattable)this.Key).ToString(format, formatProvider);");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task WithComplexGenericTypeName_GeneratesCorrectly()
   {
      // Arrange
      var state = new FormattableStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.Container<global::System.String>.Inner", "Inner")
                  .WithStringKeyMember("_id")
                  .Build();

      var sb = new StringBuilder();
      var generator = FormattableCodeGenerator.Instance;

      // Act
      generator.GenerateImplementation(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task WithSpecialCharactersInTypeName_EscapesCorrectly()
   {
      // Arrange
      var state = new FormattableStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.@Special.@Type", "Type")
                  .WithIntKeyMember("@value")
                  .Build();

      var sb = new StringBuilder();
      var generator = FormattableCodeGenerator.Instance;

      // Act
      generator.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("this.@value");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task WithPublicKeyMemberName_GeneratesCorrectly()
   {
      // Arrange
      var state = new FormattableStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.TestType")
                  .WithIntKeyMember("Value")
                  .Build();

      var sb = new StringBuilder();
      var generator = FormattableCodeGenerator.Instance;

      // Act
      generator.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("this.Value");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task WithPrivateKeyMemberName_GeneratesCorrectly()
   {
      // Arrange
      var state = new FormattableStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.TestType")
                  .WithIntKeyMember("_value")
                  .Build();

      var sb = new StringBuilder();
      var generator = FormattableCodeGenerator.Instance;

      // Act
      generator.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("this._value");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task MultipleCallsToSameStringBuilder_AppendsContent()
   {
      // Arrange
      var state1 = new FormattableStateBuilder()
                   .WithReferenceType("global::Thinktecture.Tests.Type1", "Type1")
                   .WithIntKeyMember("Value1")
                   .Build();

      var state2 = new FormattableStateBuilder()
                   .WithReferenceType("global::Thinktecture.Tests.Type2", "Type2")
                   .WithStringKeyMember("Value2")
                   .Build();

      var sb = new StringBuilder();
      var generator = FormattableCodeGenerator.Instance;

      // Act
      generator.GenerateImplementation(sb, state1);
      generator.GenerateImplementation(sb, state2);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("this.Value1");
      result.Should().Contain("this.Value2");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task IncludesInheritdocComment()
   {
      // Arrange
      var state = new FormattableStateBuilder()
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();
      var generator = FormattableCodeGenerator.Instance;

      // Act
      generator.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("/// <inheritdoc />");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task WithCustomKeyMemberType_GeneratesCorrectly()
   {
      // Arrange
      var state = new FormattableStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.CustomType")
                  .WithKeyMember("CustomKey", "global::Thinktecture.Tests.CustomKeyType")
                  .Build();

      var sb = new StringBuilder();
      var generator = FormattableCodeGenerator.Instance;

      // Act
      generator.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("this.CustomKey");
      await Verifier.Verify(result);
   }
}

using System.Text;
using System.Threading.Tasks;
using Thinktecture.CodeAnalysis;
using VerifyXunit;

namespace Thinktecture.Runtime.Tests.CodeAnalysis.SpanParsableCodeGeneratorTests;

public class GenerateImplementation
{
   [Fact]
   public void GenerateImplementation_WithEmptyStringBuilder_AppendsContent()
   {
      // Arrange
      var state = new SpanParsableStateBuilder()
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();

      // Act
      SpanParsableCodeGenerator.ForValueObject.GenerateImplementation(sb, state);

      // Assert
      sb.Length.Should().BeGreaterThan(0);
   }

   [Fact]
   public async Task ForValueObject_WithIntKey_GeneratesParseAndTryParseUsingStaticAbstractInvoker()
   {
      // Arrange
      var state = new SpanParsableStateBuilder()
                  .WithType("global::Thinktecture.Tests.IntValueObject", "IntValueObject")
                  .WithIntKeyMember("_value")
                  .Build();

      var sb = new StringBuilder();

      // Act
      SpanParsableCodeGenerator.ForValueObject.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("Parse(global::System.ReadOnlySpan<char> s");
      result.Should().Contain("TryParse(");
      result.Should().Contain("global::System.ReadOnlySpan<char> s");
      result.Should().Contain("global::Thinktecture.Internal.StaticAbstractInvoker.ParseValue<");
      result.Should().NotContain("ParseValue("); // No inline helper methods
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task ForValueObject_WithGuidKey_GeneratesParseAndTryParseUsingStaticAbstractInvoker()
   {
      // Arrange
      var state = new SpanParsableStateBuilder()
                  .WithType("global::Thinktecture.Tests.GuidValueObject", "GuidValueObject")
                  .WithGuidKeyMember("_value")
                  .Build();

      var sb = new StringBuilder();

      // Act
      SpanParsableCodeGenerator.ForValueObject.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("Parse(global::System.ReadOnlySpan<char> s");
      result.Should().Contain("TryParse(");
      result.Should().Contain("global::Thinktecture.Internal.StaticAbstractInvoker.ParseValue<global::System.Guid>");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task ForValueObject_WithDecimalKey_GeneratesParseAndTryParseUsingStaticAbstractInvoker()
   {
      // Arrange
      var state = new SpanParsableStateBuilder()
                  .WithType("global::Thinktecture.Tests.DecimalValueObject", "DecimalValueObject")
                  .WithDecimalKeyMember("_value")
                  .Build();

      var sb = new StringBuilder();

      // Act
      SpanParsableCodeGenerator.ForValueObject.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("global::Thinktecture.Internal.StaticAbstractInvoker.ParseValue<global::System.Decimal>");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task ForValueObject_WithLongKey_GeneratesParseAndTryParseUsingStaticAbstractInvoker()
   {
      // Arrange
      var state = new SpanParsableStateBuilder()
                  .WithType("global::Thinktecture.Tests.LongValueObject", "LongValueObject")
                  .WithLongKeyMember("_value")
                  .Build();

      var sb = new StringBuilder();

      // Act
      SpanParsableCodeGenerator.ForValueObject.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("global::Thinktecture.Internal.StaticAbstractInvoker.ParseValue<global::System.Int64>");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task ForValueObject_WithStringKey_GeneratesParseAndTryParseUsingStaticAbstractInvoker()
   {
      // Arrange
      var state = new SpanParsableStateBuilder()
                  .WithType("global::Thinktecture.Tests.StringValueObject", "StringValueObject")
                  .WithStringKeyMember("_value")
                  .Build();

      var sb = new StringBuilder();

      // Act
      SpanParsableCodeGenerator.ForValueObject.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("Parse(global::System.ReadOnlySpan<char> s");
      result.Should().Contain("TryParse(");
      result.Should().Contain("global::System.ReadOnlySpan<char> s");
      result.Should().Contain("global::Thinktecture.Internal.StaticAbstractInvoker.Validate<");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task ForValueObject_WithStringBasedValidateMethod_And_NonStringKey_ConvertsToString()
   {
      // Arrange
      var state = new SpanParsableStateBuilder()
                  .WithType("global::Thinktecture.Tests.MixedType", "MixedType")
                  .WithIntKeyMember("_value")
                  .WithHasReadOnlySpanOfCharBasedValidateMethod()
                  .Build();

      var sb = new StringBuilder();

      // Act
      SpanParsableCodeGenerator.ForValueObject.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().NotContain("ParseValue"); // No parsing of key type
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task Parse_ErrorMessage_ContainsTypeName()
   {
      // Arrange
      var state = new SpanParsableStateBuilder()
                  .WithType("global::Thinktecture.Tests.MyCustomType", "MyCustomType")
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();

      // Act
      SpanParsableCodeGenerator.ForValueObject.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("Unable to parse \\\"MyCustomType\\\"");
      await Verifier.Verify(result);
   }

   [Theory]
   [InlineData("global::Thinktecture.ValidationError")]
   [InlineData("global::My.Custom.ValidationError")]
   [InlineData("global::System.String")]
   public async Task DifferentValidationErrorTypes_GenerateCorrectly(string validationErrorType)
   {
      // Arrange
      var state = new SpanParsableStateBuilder()
                  .WithType("global::Thinktecture.Tests.TestType")
                  .WithIntKeyMember()
                  .WithValidationError(validationErrorType)
                  .Build();

      var sb = new StringBuilder();

      // Act
      SpanParsableCodeGenerator.ForValueObject.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain(validationErrorType);
      await Verifier.Verify(result).UseParameters(validationErrorType.Replace("global::", "").Replace(".", "_"));
   }

   [Fact]
   public async Task ForEnum_WithStringKey_GeneratesSpanMethodsWithSpecialHandling()
   {
      // Arrange
      var state = new SpanParsableStateBuilder()
                  .WithType("global::Thinktecture.Tests.StringEnum", "StringEnum")
                  .WithStringKeyMember("_key")
                  .WithIsEnum()
                  .Build();

      var sb = new StringBuilder();

      // Act
      SpanParsableCodeGenerator.ForEnum.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("Parse(global::System.ReadOnlySpan<char> s");
      result.Should().Contain("TryParse(");
      result.Should().Contain("#if NET9_0_OR_GREATER"); // String-based enums have special NET9+ handling
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task ForEnum_WithIntKey_GeneratesSpanMethodsUsingStaticAbstractInvoker()
   {
      // Arrange
      var state = new SpanParsableStateBuilder()
                  .WithType("global::Thinktecture.Tests.IntEnum", "IntEnum")
                  .WithIntKeyMember("_key")
                  .WithIsEnum()
                  .Build();

      var sb = new StringBuilder();

      // Act
      SpanParsableCodeGenerator.ForEnum.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("Parse(global::System.ReadOnlySpan<char> s");
      result.Should().Contain("TryParse(");
      result.Should().Contain("global::Thinktecture.Internal.StaticAbstractInvoker.ParseValue<global::System.Int32>");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task ForEnum_WithGuidKey_GeneratesSpanMethodsUsingStaticAbstractInvoker()
   {
      // Arrange
      var state = new SpanParsableStateBuilder()
                  .WithType("global::Thinktecture.Tests.GuidEnum", "GuidEnum")
                  .WithGuidKeyMember("_key")
                  .WithIsEnum()
                  .Build();

      var sb = new StringBuilder();

      // Act
      SpanParsableCodeGenerator.ForEnum.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("global::Thinktecture.Internal.StaticAbstractInvoker.ParseValue<global::System.Guid>");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task ForEnum_WithDecimalKey_GeneratesSpanMethodsUsingStaticAbstractInvoker()
   {
      // Arrange
      var state = new SpanParsableStateBuilder()
                  .WithType("global::Thinktecture.Tests.DecimalEnum", "DecimalEnum")
                  .WithDecimalKeyMember("_key")
                  .WithIsEnum()
                  .Build();

      var sb = new StringBuilder();

      // Act
      SpanParsableCodeGenerator.ForEnum.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("global::Thinktecture.Internal.StaticAbstractInvoker.ParseValue<global::System.Decimal>");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task TryParse_UsesTryParseForKeyParsing()
   {
      // Arrange
      var state = new SpanParsableStateBuilder()
                  .WithType("global::Thinktecture.Tests.IntValueObject", "IntValueObject")
                  .WithIntKeyMember("_value")
                  .Build();

      var sb = new StringBuilder();

      // Act
      SpanParsableCodeGenerator.ForValueObject.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("TryParse");
      result.Should().Contain("result = default;");
      result.Should().Contain("return false;");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task ForEnum_WithStringKeyAndCustomValidationError_GeneratesCorrectly()
   {
      // Arrange
      var state = new SpanParsableStateBuilder()
                  .WithType("global::Thinktecture.Tests.Country", "Country")
                  .WithStringKeyMember("_key")
                  .WithIsEnum()
                  .WithValidationError("global::Thinktecture.Tests.CountryValidationError")
                  .Build();

      var sb = new StringBuilder();

      // Act
      SpanParsableCodeGenerator.ForEnum.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task ForValueObject_WithComplexGenericTypeName_GeneratesCorrectly()
   {
      // Arrange
      var state = new SpanParsableStateBuilder()
                  .WithType("global::Thinktecture.Tests.GenericContainer<global::System.String>.InnerValueObject", "InnerValueObject")
                  .WithIntKeyMember("_value")
                  .Build();

      var sb = new StringBuilder();

      // Act
      SpanParsableCodeGenerator.ForValueObject.GenerateImplementation(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task ForValueObject_WithSpecialCharactersInTypeName_EscapesCorrectly()
   {
      // Arrange
      var state = new SpanParsableStateBuilder()
                  .WithType("global::Thinktecture.Tests.@Special.@Type", "Type")
                  .WithIntKeyMember("_value")
                  .Build();

      var sb = new StringBuilder();

      // Act
      SpanParsableCodeGenerator.ForValueObject.GenerateImplementation(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task ForEnum_WithGenericTypeAndStringKey_GeneratesCorrectly()
   {
      // Arrange
      var state = new SpanParsableStateBuilder()
                  .WithGenericType("global::Thinktecture.Tests.GenericEnum", "GenericEnum", "<T>", "where T : class")
                  .WithStringKeyMember("_key")
                  .WithIsEnum()
                  .Build();

      var sb = new StringBuilder();

      // Act
      SpanParsableCodeGenerator.ForEnum.GenerateImplementation(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task ForEnum_WithGenericTypeAndIntKey_GeneratesCorrectly()
   {
      // Arrange
      var state = new SpanParsableStateBuilder()
                  .WithGenericType("global::Thinktecture.Tests.GenericIntEnum", "GenericIntEnum", "<T>", "where T : System.IEquatable<T>")
                  .WithIntKeyMember("_key")
                  .WithIsEnum()
                  .Build();

      var sb = new StringBuilder();

      // Act
      SpanParsableCodeGenerator.ForEnum.GenerateImplementation(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task ForValueObject_WithGenericTypeAndStringKey_GeneratesCorrectly()
   {
      // Arrange
      var state = new SpanParsableStateBuilder()
                  .WithGenericType("global::Thinktecture.Tests.GenericValueObject", "GenericValueObject", "<TKey, TValue>", "where TKey : notnull where TValue : class")
                  .WithStringKeyMember("_value")
                  .Build();

      var sb = new StringBuilder();

      // Act
      SpanParsableCodeGenerator.ForValueObject.GenerateImplementation(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task ForValueObject_WithGenericTypeAndIntKey_GeneratesCorrectly()
   {
      // Arrange
      var state = new SpanParsableStateBuilder()
                  .WithGenericType("global::Thinktecture.Tests.GenericIntValueObject", "GenericIntValueObject", "<T>", "where T : System.IComparable<T>")
                  .WithIntKeyMember("_value")
                  .Build();

      var sb = new StringBuilder();

      // Act
      SpanParsableCodeGenerator.ForValueObject.GenerateImplementation(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task Parse_WithNET9Conditionals_GeneratesCorrectly()
   {
      // Arrange
      var state = new SpanParsableStateBuilder()
                  .WithType("global::Thinktecture.Tests.IntValueObject", "IntValueObject")
                  .WithIntKeyMember("_value")
                  .Build();

      var sb = new StringBuilder();

      // Act
      SpanParsableCodeGenerator.ForValueObject.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("#if NET9_0_OR_GREATER");
      result.Should().Contain("#endif");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task TryParse_WithNET9Conditionals_GeneratesCorrectly()
   {
      // Arrange
      var state = new SpanParsableStateBuilder()
                  .WithType("global::Thinktecture.Tests.GuidValueObject", "GuidValueObject")
                  .WithGuidKeyMember("_value")
                  .Build();

      var sb = new StringBuilder();

      // Act
      SpanParsableCodeGenerator.ForValueObject.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("#if NET9_0_OR_GREATER");
      await Verifier.Verify(result);
   }
}

using System.Text;
using System.Threading.Tasks;
using Thinktecture.CodeAnalysis;
using VerifyXunit;

namespace Thinktecture.Runtime.Tests.CodeAnalysis.ParsableCodeGeneratorTests;

public class GenerateImplementation
{
   [Fact]
   public void GenerateImplementation_WithEmptyStringBuilder_AppendsContent()
   {
      // Arrange
      var state = new ParsableStateBuilder()
                  .WithStringKeyMember()
                  .Build();

      var sb = new StringBuilder();

      // Act
      ParsableCodeGenerator.ForValueObject.GenerateImplementation(sb, state);

      // Assert
      sb.Length.Should().BeGreaterThan(0);
   }

   [Fact]
   public async Task ForEnum_WithStringKey_DoesNotGenerateSpanMethodsForValueObject()
   {
      // Arrange - Create state with string key but NOT marked as enum
      var state = new ParsableStateBuilder()
                  .WithType("global::Thinktecture.Tests.TestType")
                  .WithStringKeyMember()
                  .Build(); // IsEnum is false

      var sb = new StringBuilder();

      // Act - Use ForValueObject generator
      ParsableCodeGenerator.ForValueObject.GenerateImplementation(sb, state);

      // Assert - Should NOT contain ReadOnlySpan methods
      var result = sb.ToString();
      result.Should().NotContain("ReadOnlySpan<char>");
      result.Should().NotContain("#if NET9_0_OR_GREATER");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task ForValueObject_WithStringBasedValidateMethod_And_NonStringKey_UsesStringValidation()
   {
      // Arrange - Int key with string-based validate method
      var state = new ParsableStateBuilder()
                  .WithType("global::Thinktecture.Tests.MixedType", "MixedType")
                  .WithIntKeyMember("_value")
                  .WithHasStringBasedValidateMethod()
                  .Build();

      var sb = new StringBuilder();

      // Act
      ParsableCodeGenerator.ForValueObject.GenerateImplementation(sb, state);

      // Assert - Should use string directly, not parse int first
      var result = sb.ToString();
      result.Should().NotContain("ParseValue");
      result.Should().Contain("Validate<global::Thinktecture.Tests.MixedType>(s, provider, out var result)");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task ForEnum_WithNonParsableKeyType_GeneratesParseValueHelper()
   {
      // Arrange - Enum with Guid key (which is IParsable but needs ParseValue helper)
      var state = new ParsableStateBuilder()
                  .WithType("global::Thinktecture.Tests.GuidEnum", "GuidEnum")
                  .WithGuidKeyMember("_key")
                  .WithIsEnum()
                  .Build();

      var sb = new StringBuilder();

      // Act
      ParsableCodeGenerator.ForEnum.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("ParseValue");
      result.Should().Contain("TryParseValue");
      result.Should().NotContain("ReadOnlySpan<char>");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task Parse_ErrorMessage_ContainsTypeName()
   {
      // Arrange
      var state = new ParsableStateBuilder()
                  .WithType("global::Thinktecture.Tests.MyCustomType", "MyCustomType")
                  .WithStringKeyMember()
                  .Build();

      var sb = new StringBuilder();

      // Act
      ParsableCodeGenerator.ForValueObject.GenerateImplementation(sb, state);

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
      var state = new ParsableStateBuilder()
                  .WithType("global::Thinktecture.Tests.TestType")
                  .WithStringKeyMember()
                  .WithValidationError(validationErrorType)
                  .Build();

      var sb = new StringBuilder();

      // Act
      ParsableCodeGenerator.ForValueObject.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain(validationErrorType);
      await Verifier.Verify(result).UseParameters(validationErrorType.Replace("global::", "").Replace(".", "_"));
   }

   [Fact]
   public async Task ForValueObject_WithStringKey_GeneratesStringBasedMethods()
   {
      // Arrange
      var state = new ParsableStateBuilder()
                  .WithType("global::Thinktecture.Tests.ProductId", "ProductId")
                  .WithStringKeyMember("_value")
                  .Build();

      var sb = new StringBuilder();

      // Act
      ParsableCodeGenerator.ForValueObject.GenerateImplementation(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task ForValueObject_WithIntKey_GeneratesParseValueHelpers()
   {
      // Arrange
      var state = new ParsableStateBuilder()
                  .WithType("global::Thinktecture.Tests.Amount", "Amount")
                  .WithIntKeyMember("_value")
                  .Build();

      var sb = new StringBuilder();

      // Act
      ParsableCodeGenerator.ForValueObject.GenerateImplementation(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task ForValueObject_WithGuidKey_GeneratesParseValueHelpers()
   {
      // Arrange
      var state = new ParsableStateBuilder()
                  .WithType("global::Thinktecture.Tests.OrderId", "OrderId")
                  .WithGuidKeyMember("_value")
                  .Build();

      var sb = new StringBuilder();

      // Act
      ParsableCodeGenerator.ForValueObject.GenerateImplementation(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task ForValueObject_WithStringBasedValidateMethod_TreatsAsString()
   {
      // Arrange
      var state = new ParsableStateBuilder()
                  .WithType("global::Thinktecture.Tests.CustomType", "CustomType")
                  .WithIntKeyMember("_value")
                  .WithHasStringBasedValidateMethod()
                  .Build();

      var sb = new StringBuilder();

      // Act
      ParsableCodeGenerator.ForValueObject.GenerateImplementation(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task ForValueObject_WithCustomValidationError_UsesCustomType()
   {
      // Arrange
      var state = new ParsableStateBuilder()
                  .WithType("global::Thinktecture.Tests.Email", "Email")
                  .WithStringKeyMember("_value")
                  .WithValidationError("global::Thinktecture.Tests.EmailValidationError")
                  .Build();

      var sb = new StringBuilder();

      // Act
      ParsableCodeGenerator.ForValueObject.GenerateImplementation(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task ForEnum_WithStringKey_GeneratesAllMethodsIncludingSpanVariants()
   {
      // Arrange
      var state = new ParsableStateBuilder()
                  .WithType("global::Thinktecture.Tests.ProductGroup", "ProductGroup")
                  .WithStringKeyMember("_key")
                  .WithIsEnum()
                  .Build();

      var sb = new StringBuilder();

      // Act
      ParsableCodeGenerator.ForEnum.GenerateImplementation(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task ForEnum_WithIntKey_GeneratesStringMethodsOnly()
   {
      // Arrange
      var state = new ParsableStateBuilder()
                  .WithType("global::Thinktecture.Tests.StatusCode", "StatusCode")
                  .WithIntKeyMember("_key")
                  .WithIsEnum()
                  .Build();

      var sb = new StringBuilder();

      // Act
      ParsableCodeGenerator.ForEnum.GenerateImplementation(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task ForEnum_WithGuidKey_GeneratesStringMethodsOnly()
   {
      // Arrange
      var state = new ParsableStateBuilder()
                  .WithType("global::Thinktecture.Tests.EntityId", "EntityId")
                  .WithGuidKeyMember("_key")
                  .WithIsEnum()
                  .Build();

      var sb = new StringBuilder();

      // Act
      ParsableCodeGenerator.ForEnum.GenerateImplementation(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task ForEnum_WithStringKeyAndCustomValidationError_GeneratesCorrectly()
   {
      // Arrange
      var state = new ParsableStateBuilder()
                  .WithType("global::Thinktecture.Tests.Country", "Country")
                  .WithStringKeyMember("_key")
                  .WithIsEnum()
                  .WithValidationError("global::Thinktecture.Tests.CountryValidationError")
                  .Build();

      var sb = new StringBuilder();

      // Act
      ParsableCodeGenerator.ForEnum.GenerateImplementation(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task ForValueObject_WithoutKeyMember_GeneratesCorrectly()
   {
      // Arrange
      var state = new ParsableStateBuilder()
                  .WithType("global::Thinktecture.Tests.ComplexValue", "ComplexValue")
                  .WithoutKeyMember()
                  .Build();

      var sb = new StringBuilder();

      // Act
      ParsableCodeGenerator.ForValueObject.GenerateImplementation(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task ForEnum_WithoutKeyMember_GeneratesCorrectly()
   {
      // Arrange
      var state = new ParsableStateBuilder()
                  .WithType("global::Thinktecture.Tests.KeylessEnum", "KeylessEnum")
                  .WithoutKeyMember()
                  .WithIsEnum()
                  .Build();

      var sb = new StringBuilder();

      // Act
      ParsableCodeGenerator.ForEnum.GenerateImplementation(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task ForEnum_WithComplexGenericTypeName_GeneratesCorrectly()
   {
      // Arrange
      var state = new ParsableStateBuilder()
                  .WithType("global::Thinktecture.Tests.GenericContainer<global::System.String>.InnerEnum", "InnerEnum")
                  .WithStringKeyMember("_key")
                  .WithIsEnum()
                  .Build();

      var sb = new StringBuilder();

      // Act
      ParsableCodeGenerator.ForEnum.GenerateImplementation(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task ForValueObject_WithSpecialCharactersInTypeName_EscapesCorrectly()
   {
      // Arrange
      var state = new ParsableStateBuilder()
                  .WithType("global::Thinktecture.Tests.@Special.@Type", "Type")
                  .WithStringKeyMember("_value")
                  .Build();

      var sb = new StringBuilder();

      // Act
      ParsableCodeGenerator.ForValueObject.GenerateImplementation(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task ForEnum_WithGenericTypeAndStringKey_GeneratesCorrectly()
   {
      // Arrange
      var state = new ParsableStateBuilder()
                  .WithGenericType("global::Thinktecture.Tests.GenericEnum", "GenericEnum", "<T>", "where T : class")
                  .WithStringKeyMember("_key")
                  .WithIsEnum()
                  .Build();

      var sb = new System.Text.StringBuilder();

      // Act
      ParsableCodeGenerator.ForEnum.GenerateImplementation(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task ForEnum_WithGenericTypeAndIntKey_GeneratesCorrectly()
   {
      // Arrange
      var state = new ParsableStateBuilder()
                  .WithGenericType("global::Thinktecture.Tests.GenericIntEnum", "GenericIntEnum", "<T>", "where T : System.IEquatable<T>")
                  .WithIntKeyMember("_key")
                  .WithIsEnum()
                  .Build();

      var sb = new System.Text.StringBuilder();

      // Act
      ParsableCodeGenerator.ForEnum.GenerateImplementation(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task ForValueObject_WithGenericTypeAndStringKey_GeneratesCorrectly()
   {
      // Arrange
      var state = new ParsableStateBuilder()
                  .WithGenericType("global::Thinktecture.Tests.GenericValueObject", "GenericValueObject", "<TKey, TValue>", "where TKey : notnull where TValue : class")
                  .WithStringKeyMember("_value")
                  .Build();

      var sb = new System.Text.StringBuilder();

      // Act
      ParsableCodeGenerator.ForValueObject.GenerateImplementation(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task ForValueObject_WithGenericTypeAndIntKey_GeneratesCorrectly()
   {
      // Arrange
      var state = new ParsableStateBuilder()
                  .WithGenericType("global::Thinktecture.Tests.GenericIntValueObject", "GenericIntValueObject", "<T>", "where T : System.IComparable<T>")
                  .WithIntKeyMember("_value")
                  .Build();

      var sb = new System.Text.StringBuilder();

      // Act
      ParsableCodeGenerator.ForValueObject.GenerateImplementation(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }
}

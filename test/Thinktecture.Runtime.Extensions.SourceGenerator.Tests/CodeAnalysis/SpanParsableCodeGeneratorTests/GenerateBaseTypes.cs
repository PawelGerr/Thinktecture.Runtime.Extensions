using System.Text;
using System.Threading.Tasks;
using Thinktecture.CodeAnalysis;
using VerifyXunit;

namespace Thinktecture.Runtime.Tests.CodeAnalysis.SpanParsableCodeGeneratorTests;

public class GenerateBaseTypes
{
   [Fact]
   public void GenerateBaseTypes_WithEmptyStringBuilder_AppendsContent()
   {
      // Arrange
      var state = new SpanParsableStateBuilder()
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();

      // Act
      SpanParsableCodeGenerator.ForValueObject.GenerateBaseTypes(sb, state);

      // Assert
      sb.Length.Should().BeGreaterThan(0);
   }

   [Fact]
   public async Task GenerateBaseTypes_MultipleCallsToSameStringBuilder_AppendsContent()
   {
      // Arrange
      var state1 = new SpanParsableStateBuilder()
                   .WithType("global::Thinktecture.Tests.Type1", "Type1")
                   .WithIntKeyMember()
                   .Build();

      var state2 = new SpanParsableStateBuilder()
                   .WithType("global::Thinktecture.Tests.Type2", "Type2")
                   .WithGuidKeyMember()
                   .WithIsEnum()
                   .Build();

      var sb = new StringBuilder();

      // Act
      SpanParsableCodeGenerator.ForValueObject.GenerateBaseTypes(sb, state1);
      SpanParsableCodeGenerator.ForEnum.GenerateBaseTypes(sb, state2);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("Type1");
      result.Should().Contain("Type2");
      result.Should().Contain("ISpanParsable");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task ForValueObject_WithIntKey_GeneratesISpanParsable()
   {
      // Arrange
      var state = new SpanParsableStateBuilder()
                  .WithType("global::Thinktecture.Tests.IntValueObject", "IntValueObject")
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();

      // Act
      SpanParsableCodeGenerator.ForValueObject.GenerateBaseTypes(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("ISpanParsable<");
      result.Should().Contain("IntValueObject");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task ForValueObject_WithGuidKey_GeneratesISpanParsable()
   {
      // Arrange
      var state = new SpanParsableStateBuilder()
                  .WithType("global::Thinktecture.Tests.GuidValueObject", "GuidValueObject")
                  .WithGuidKeyMember()
                  .Build();

      var sb = new StringBuilder();

      // Act
      SpanParsableCodeGenerator.ForValueObject.GenerateBaseTypes(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("ISpanParsable<");
      result.Should().Contain("GuidValueObject");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task ForValueObject_WithDecimalKey_GeneratesISpanParsable()
   {
      // Arrange
      var state = new SpanParsableStateBuilder()
                  .WithType("global::Thinktecture.Tests.DecimalValueObject", "DecimalValueObject")
                  .WithDecimalKeyMember()
                  .Build();

      var sb = new StringBuilder();

      // Act
      SpanParsableCodeGenerator.ForValueObject.GenerateBaseTypes(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("ISpanParsable<");
      result.Should().Contain("DecimalValueObject");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task ForValueObject_WithLongKey_GeneratesISpanParsable()
   {
      // Arrange
      var state = new SpanParsableStateBuilder()
                  .WithType("global::Thinktecture.Tests.LongValueObject", "LongValueObject")
                  .WithLongKeyMember()
                  .Build();

      var sb = new StringBuilder();

      // Act
      SpanParsableCodeGenerator.ForValueObject.GenerateBaseTypes(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("ISpanParsable<");
      result.Should().Contain("LongValueObject");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task ForValueObject_WithStringKey_GeneratesISpanParsable()
   {
      // Arrange
      var state = new SpanParsableStateBuilder()
                  .WithType("global::Thinktecture.Tests.StringValueObject", "StringValueObject")
                  .WithStringKeyMember()
                  .Build();

      var sb = new StringBuilder();

      // Act
      SpanParsableCodeGenerator.ForValueObject.GenerateBaseTypes(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("ISpanParsable<");
      result.Should().Contain("StringValueObject");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task ForEnum_WithStringKey_GeneratesISpanParsable()
   {
      // Arrange
      var state = new SpanParsableStateBuilder()
                  .WithType("global::Thinktecture.Tests.StringEnum", "StringEnum")
                  .WithStringKeyMember()
                  .WithIsEnum()
                  .Build();

      var sb = new StringBuilder();

      // Act
      SpanParsableCodeGenerator.ForEnum.GenerateBaseTypes(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("ISpanParsable<");
      result.Should().Contain("StringEnum");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task ForEnum_WithIntKey_GeneratesISpanParsable()
   {
      // Arrange
      var state = new SpanParsableStateBuilder()
                  .WithType("global::Thinktecture.Tests.IntEnum", "IntEnum")
                  .WithIntKeyMember()
                  .WithIsEnum()
                  .Build();

      var sb = new StringBuilder();

      // Act
      SpanParsableCodeGenerator.ForEnum.GenerateBaseTypes(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("ISpanParsable<");
      result.Should().Contain("IntEnum");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task ForEnum_WithGuidKey_GeneratesISpanParsable()
   {
      // Arrange
      var state = new SpanParsableStateBuilder()
                  .WithType("global::Thinktecture.Tests.GuidEnum", "GuidEnum")
                  .WithGuidKeyMember()
                  .WithIsEnum()
                  .Build();

      var sb = new StringBuilder();

      // Act
      SpanParsableCodeGenerator.ForEnum.GenerateBaseTypes(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("ISpanParsable<");
      result.Should().Contain("GuidEnum");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task ForEnum_WithDecimalKey_GeneratesISpanParsable()
   {
      // Arrange
      var state = new SpanParsableStateBuilder()
                  .WithType("global::Thinktecture.Tests.DecimalEnum", "DecimalEnum")
                  .WithDecimalKeyMember()
                  .WithIsEnum()
                  .Build();

      var sb = new StringBuilder();

      // Act
      SpanParsableCodeGenerator.ForEnum.GenerateBaseTypes(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("ISpanParsable<");
      result.Should().Contain("DecimalEnum");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task ForEnum_WithDateTimeKey_GeneratesISpanParsable()
   {
      // Arrange
      var state = new SpanParsableStateBuilder()
                  .WithType("global::Thinktecture.Tests.DateTimeEnum", "DateTimeEnum")
                  .WithDateTimeKeyMember()
                  .WithIsEnum()
                  .Build();

      var sb = new StringBuilder();

      // Act
      SpanParsableCodeGenerator.ForEnum.GenerateBaseTypes(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("ISpanParsable<");
      result.Should().Contain("DateTimeEnum");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task ForEnum_WithComplexTypeName_EscapesCorrectly()
   {
      // Arrange
      var state = new SpanParsableStateBuilder()
                  .WithType("global::Thinktecture.Tests.Nested.Complex<T>.InnerEnum", "InnerEnum")
                  .WithStringKeyMember()
                  .WithIsEnum()
                  .Build();

      var sb = new StringBuilder();

      // Act
      SpanParsableCodeGenerator.ForEnum.GenerateBaseTypes(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task ForValueObject_WithGenericType_GeneratesCorrectly()
   {
      // Arrange
      var state = new SpanParsableStateBuilder()
                  .WithGenericType("global::Thinktecture.Tests.GenericValueObject", "GenericValueObject", "<T>", "where T : class")
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();

      // Act
      SpanParsableCodeGenerator.ForValueObject.GenerateBaseTypes(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }
}

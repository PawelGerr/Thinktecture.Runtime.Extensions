using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Thinktecture.CodeAnalysis;
using VerifyXunit;

namespace Thinktecture.Runtime.Tests.CodeAnalysis.KeyedJsonCodeGeneratorTests;

public class Generate
{
   [Fact]
   public void Should_generate_nothing_when_keyType_is_null()
   {
      // Arrange
      var state = new KeyedJsonCodeGeneratorStateBuilder()
                  .WithoutKeyMember()
                  .Build();

      var sb = new StringBuilder();
      var generator = new KeyedJsonCodeGenerator(state, sb);

      // Act
      generator.Generate(CancellationToken.None);

      // Assert
      sb.Length.Should().Be(0);
   }

   [Fact]
   public void Should_generate_nothing_when_no_custom_factory_and_no_key_member()
   {
      // Arrange
      var state = new KeyedJsonCodeGeneratorStateBuilder()
                  .WithoutKeyMember()
                  .Build();

      var sb = new StringBuilder();
      var generator = new KeyedJsonCodeGenerator(state, sb);

      // Act
      generator.Generate(CancellationToken.None);

      // Assert
      sb.Length.Should().Be(0);
   }

   [Fact]
   public void Should_use_custom_factory_type_when_available_for_json()
   {
      // Arrange
      var state = new KeyedJsonCodeGeneratorStateBuilder()
                  .WithIntKeyMember() // Has key member
                  .WithCustomFactory("global::MyNamespace.MyCustomType", useForSerialization: global::Thinktecture.CodeAnalysis.SerializationFrameworks.SystemTextJson)
                  .Build();

      var sb = new StringBuilder();
      var generator = new KeyedJsonCodeGenerator(state, sb);

      // Act
      generator.Generate(CancellationToken.None);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("MyNamespace.MyCustomType");
      result.Should().NotContain("System.Int32"); // Should not use key member type
   }

   [Fact]
   public void Should_use_key_member_when_custom_factory_exists_but_not_for_json()
   {
      // Arrange
      var state = new KeyedJsonCodeGeneratorStateBuilder()
                  .WithIntKeyMember()
                  .WithCustomFactory("global::MyNamespace.MyCustomType", useForSerialization: global::Thinktecture.CodeAnalysis.SerializationFrameworks.MessagePack) // Not for JSON
                  .Build();

      var sb = new StringBuilder();
      var generator = new KeyedJsonCodeGenerator(state, sb);

      // Act
      generator.Generate(CancellationToken.None);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("global::System.Int32"); // Should use key member type
   }

   [Fact]
   public void Should_generate_with_two_params_for_string_key_from_key_member()
   {
      // Arrange
      var state = new KeyedJsonCodeGeneratorStateBuilder()
                  .WithStringKeyMember()
                  .Build();

      var sb = new StringBuilder();
      var generator = new KeyedJsonCodeGenerator(state, sb);

      // Act
      generator.Generate(CancellationToken.None);

      // Assert
      var result = sb.ToString();
      // For string, should have 2 parameters: TType and TValidationError (no key type parameter)
      result.Should().Contain("ThinktectureJsonConverterFactory<global::Thinktecture.Tests.TestType, global::Thinktecture.ValidationError>");
      result.Should().NotContain("global::System.String,"); // Key type should not be in parameters for string
   }

   [Fact]
   public void Should_generate_with_two_params_for_string_custom_factory()
   {
      // Arrange
      var state = new KeyedJsonCodeGeneratorStateBuilder()
                  .WithoutKeyMember()
                  .WithStringCustomFactory()
                  .Build();

      var sb = new StringBuilder();
      var generator = new KeyedJsonCodeGenerator(state, sb);

      // Act
      generator.Generate(CancellationToken.None);

      // Assert
      var result = sb.ToString();
      // For string, should have 2 parameters
      result.Should().Contain("ThinktectureJsonConverterFactory<global::Thinktecture.Tests.TestType, global::Thinktecture.ValidationError>");
   }

   [Fact]
   public void Should_generate_with_three_params_for_non_string_key()
   {
      // Arrange
      var state = new KeyedJsonCodeGeneratorStateBuilder()
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();
      var generator = new KeyedJsonCodeGenerator(state, sb);

      // Act
      generator.Generate(CancellationToken.None);

      // Assert
      var result = sb.ToString();
      // For non-string, should have 3 parameters: TType, TKey, and TValidationError
      result.Should().Contain("ThinktectureJsonConverterFactory<global::Thinktecture.Tests.TestType, global::System.Int32, global::Thinktecture.ValidationError>");
   }

   [Fact]
   public void Should_generate_with_three_params_for_guid_key()
   {
      // Arrange
      var state = new KeyedJsonCodeGeneratorStateBuilder()
                  .WithGuidKeyMember()
                  .Build();

      var sb = new StringBuilder();
      var generator = new KeyedJsonCodeGenerator(state, sb);

      // Act
      generator.Generate(CancellationToken.None);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("ThinktectureJsonConverterFactory<global::Thinktecture.Tests.TestType, global::System.Guid, global::Thinktecture.ValidationError>");
   }

   [Fact]
   public void Should_omit_namespace_when_type_has_no_namespace()
   {
      // Arrange
      var state = new KeyedJsonCodeGeneratorStateBuilder()
                  .WithTypeWithoutNamespace()
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();
      var generator = new KeyedJsonCodeGenerator(state, sb);

      // Act
      generator.Generate(CancellationToken.None);

      // Assert
      var result = sb.ToString();
      result.Should().NotContain("namespace ");
      result.Should().Contain("partial class TestType");
   }

   [Fact]
   public void Should_include_namespace_when_type_has_namespace()
   {
      // Arrange
      var state = new KeyedJsonCodeGeneratorStateBuilder()
                  .WithType("global::Thinktecture.Tests.TestType")
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();
      var generator = new KeyedJsonCodeGenerator(state, sb);

      // Act
      generator.Generate(CancellationToken.None);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("namespace Thinktecture.Tests;");
   }

   [Fact]
   public void Should_handle_nested_types()
   {
      // Arrange
      var containingType = new ContainingTypeState("OuterClass", true, false, default);
      var state = new KeyedJsonCodeGeneratorStateBuilder()
                  .WithNestedType(
                     typeFullyQualified: "global::Thinktecture.Tests.OuterClass.TestType",
                     name: "TestType",
                     @namespace: "Thinktecture.Tests",
                     containingType)
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();
      var generator = new KeyedJsonCodeGenerator(state, sb);

      // Act
      generator.Generate(CancellationToken.None);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("partial class OuterClass");
      result.Should().Contain("partial class TestType");
      result.Should().Contain("}"); // Closing braces for nested types
   }

   [Fact]
   public void Should_handle_multiple_nesting_levels()
   {
      // Arrange
      var outerType = new ContainingTypeState("OuterClass", true, false, default);
      var middleType = new ContainingTypeState("MiddleClass", true, false, default);
      var state = new KeyedJsonCodeGeneratorStateBuilder()
                  .WithNestedType(
                     typeFullyQualified: "global::Thinktecture.Tests.OuterClass.MiddleClass.TestType",
                     name: "TestType",
                     @namespace: "Thinktecture.Tests",
                     outerType, middleType)
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();
      var generator = new KeyedJsonCodeGenerator(state, sb);

      // Act
      generator.Generate(CancellationToken.None);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("partial class OuterClass");
      result.Should().Contain("partial class MiddleClass");
      result.Should().Contain("partial class TestType");
   }

   [Fact]
   public void Should_use_custom_validation_error_type()
   {
      // Arrange
      var state = new KeyedJsonCodeGeneratorStateBuilder()
                  .WithIntKeyMember()
                  .WithValidationError("global::MyNamespace.MyCustomValidationError")
                  .Build();

      var sb = new StringBuilder();
      var generator = new KeyedJsonCodeGenerator(state, sb);

      // Act
      generator.Generate(CancellationToken.None);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("global::MyNamespace.MyCustomValidationError>");
      result.Should().NotContain("global::Thinktecture.ValidationError>");
   }

   [Fact]
   public void Should_include_generated_code_prefix()
   {
      // Arrange
      var state = new KeyedJsonCodeGeneratorStateBuilder()
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();
      var generator = new KeyedJsonCodeGenerator(state, sb);

      // Act
      generator.Generate(CancellationToken.None);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("// <auto-generated />");
      result.Should().Contain("#nullable enable");
   }

   [Fact]
   public void Should_apply_JsonConverterAttribute()
   {
      // Arrange
      var state = new KeyedJsonCodeGeneratorStateBuilder()
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();
      var generator = new KeyedJsonCodeGenerator(state, sb);

      // Act
      generator.Generate(CancellationToken.None);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("[global::System.Text.Json.Serialization.JsonConverterAttribute(typeof(");
      result.Should().Contain("ThinktectureJsonConverterFactory");
   }

   // ===== Snapshot Tests =====

   [Fact]
   public async Task Snapshot_SimpleSmartEnumWithIntKey()
   {
      // Arrange
      var state = new KeyedJsonCodeGeneratorStateBuilder()
                  .WithType("global::Thinktecture.Tests.ProductStatus", "ProductStatus")
                  .WithIntKeyMember("Value")
                  .Build();

      var sb = new StringBuilder();
      var generator = new KeyedJsonCodeGenerator(state, sb);

      // Act
      generator.Generate(CancellationToken.None);

      // Assert
      var result = sb.ToString();
      await Verifier.Verify(result).UseMethodName("Snapshot_SimpleSmartEnumWithIntKey");
   }

   [Fact]
   public async Task Snapshot_SmartEnumWithStringKey()
   {
      // Arrange
      var state = new KeyedJsonCodeGeneratorStateBuilder()
                  .WithType("global::Thinktecture.Tests.ShippingMethod", "ShippingMethod")
                  .WithStringKeyMember("Code")
                  .Build();

      var sb = new StringBuilder();
      var generator = new KeyedJsonCodeGenerator(state, sb);

      // Act
      generator.Generate(CancellationToken.None);

      // Assert
      var result = sb.ToString();
      await Verifier.Verify(result).UseMethodName("Snapshot_SmartEnumWithStringKey");
   }

   [Fact]
   public async Task Snapshot_ValueObjectWithGuidKey()
   {
      // Arrange
      var state = new KeyedJsonCodeGeneratorStateBuilder()
                  .WithType("global::Thinktecture.Tests.ProductId", "ProductId")
                  .WithGuidKeyMember("Value")
                  .Build();

      var sb = new StringBuilder();
      var generator = new KeyedJsonCodeGenerator(state, sb);

      // Act
      generator.Generate(CancellationToken.None);

      // Assert
      var result = sb.ToString();
      await Verifier.Verify(result).UseMethodName("Snapshot_ValueObjectWithGuidKey");
   }

   [Fact]
   public async Task Snapshot_ValueObjectWithDecimalKey()
   {
      // Arrange
      var state = new KeyedJsonCodeGeneratorStateBuilder()
                  .WithType("global::Thinktecture.Tests.Amount", "Amount")
                  .WithDecimalKeyMember("Value")
                  .Build();

      var sb = new StringBuilder();
      var generator = new KeyedJsonCodeGenerator(state, sb);

      // Act
      generator.Generate(CancellationToken.None);

      // Assert
      var result = sb.ToString();
      await Verifier.Verify(result).UseMethodName("Snapshot_ValueObjectWithDecimalKey");
   }

   [Fact]
   public async Task Snapshot_NestedTypeInSingleContainer()
   {
      // Arrange
      var containingType = new ContainingTypeState("ProductModule", true, false, default);
      var state = new KeyedJsonCodeGeneratorStateBuilder()
                  .WithNestedType(
                     typeFullyQualified: "global::Thinktecture.Tests.ProductModule.ProductStatus",
                     name: "ProductStatus",
                     @namespace: "Thinktecture.Tests",
                     containingType)
                  .WithIntKeyMember("Value")
                  .Build();

      var sb = new StringBuilder();
      var generator = new KeyedJsonCodeGenerator(state, sb);

      // Act
      generator.Generate(CancellationToken.None);

      // Assert
      var result = sb.ToString();
      await Verifier.Verify(result).UseMethodName("Snapshot_NestedTypeInSingleContainer");
   }

   [Fact]
   public async Task Snapshot_DeeplyNestedType()
   {
      // Arrange
      var level1 = new ContainingTypeState("Level1", true, false, default);
      var level2 = new ContainingTypeState("Level2", true, false, default);
      var level3 = new ContainingTypeState("Level3", true, false, default);
      var state = new KeyedJsonCodeGeneratorStateBuilder()
                  .WithNestedType(
                     typeFullyQualified: "global::Thinktecture.Tests.Level1.Level2.Level3.DeepType",
                     name: "DeepType",
                     @namespace: "Thinktecture.Tests",
                     level1, level2, level3)
                  .WithIntKeyMember("Value")
                  .Build();

      var sb = new StringBuilder();
      var generator = new KeyedJsonCodeGenerator(state, sb);

      // Act
      generator.Generate(CancellationToken.None);

      // Assert
      var result = sb.ToString();
      await Verifier.Verify(result).UseMethodName("Snapshot_DeeplyNestedType");
   }

   [Fact]
   public async Task Snapshot_CustomFactoryWithStringType()
   {
      // Arrange
      var state = new KeyedJsonCodeGeneratorStateBuilder()
                  .WithType("global::Thinktecture.Tests.ISBN", "ISBN")
                  .WithoutKeyMember()
                  .WithStringCustomFactory()
                  .Build();

      var sb = new StringBuilder();
      var generator = new KeyedJsonCodeGenerator(state, sb);

      // Act
      generator.Generate(CancellationToken.None);

      // Assert
      var result = sb.ToString();
      await Verifier.Verify(result).UseMethodName("Snapshot_CustomFactoryWithStringType");
   }

   [Fact]
   public async Task Snapshot_CustomFactoryWithCustomType()
   {
      // Arrange
      var state = new KeyedJsonCodeGeneratorStateBuilder()
                  .WithType("global::Thinktecture.Tests.SpecialValue", "SpecialValue")
                  .WithoutKeyMember()
                  .WithCustomFactory("global::MyNamespace.CustomKeyType")
                  .Build();

      var sb = new StringBuilder();
      var generator = new KeyedJsonCodeGenerator(state, sb);

      // Act
      generator.Generate(CancellationToken.None);

      // Assert
      var result = sb.ToString();
      await Verifier.Verify(result).UseMethodName("Snapshot_CustomFactoryWithCustomType");
   }

   [Fact]
   public async Task Snapshot_TypeWithoutNamespace()
   {
      // Arrange
      var state = new KeyedJsonCodeGeneratorStateBuilder()
                  .WithTypeWithoutNamespace("global::GlobalType", "GlobalType")
                  .WithIntKeyMember("Value")
                  .Build();

      var sb = new StringBuilder();
      var generator = new KeyedJsonCodeGenerator(state, sb);

      // Act
      generator.Generate(CancellationToken.None);

      // Assert
      var result = sb.ToString();
      await Verifier.Verify(result).UseMethodName("Snapshot_TypeWithoutNamespace");
   }

   [Fact]
   public async Task Snapshot_CustomValidationErrorType()
   {
      // Arrange
      var state = new KeyedJsonCodeGeneratorStateBuilder()
                  .WithType("global::Thinktecture.Tests.ValidatedValue", "ValidatedValue")
                  .WithIntKeyMember("Value")
                  .WithValidationError("global::MyApp.Validation.CustomValidationError")
                  .Build();

      var sb = new StringBuilder();
      var generator = new KeyedJsonCodeGenerator(state, sb);

      // Act
      generator.Generate(CancellationToken.None);

      // Assert
      var result = sb.ToString();
      await Verifier.Verify(result).UseMethodName("Snapshot_CustomValidationErrorType");
   }
}

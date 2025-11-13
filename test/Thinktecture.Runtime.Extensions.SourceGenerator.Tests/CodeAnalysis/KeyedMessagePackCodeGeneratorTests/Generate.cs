using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Thinktecture.CodeAnalysis;
using VerifyXunit;

namespace Thinktecture.Runtime.Tests.CodeAnalysis.KeyedMessagePackCodeGeneratorTests;

public class Generate
{
   [Fact]
   public void Should_generate_nothing_when_keyType_is_null()
   {
      // Arrange
      var state = new KeyedMessagePackCodeGeneratorStateBuilder()
                  .WithoutKeyMember()
                  .Build();

      var sb = new StringBuilder();
      var generator = new KeyedMessagePackCodeGenerator(state, sb);

      // Act
      generator.Generate(CancellationToken.None);

      // Assert
      sb.Length.Should().Be(0);
   }

   [Fact]
   public void Should_generate_nothing_when_no_custom_factory_and_no_key_member()
   {
      // Arrange
      var state = new KeyedMessagePackCodeGeneratorStateBuilder()
                  .WithoutKeyMember()
                  .Build();

      var sb = new StringBuilder();
      var generator = new KeyedMessagePackCodeGenerator(state, sb);

      // Act
      generator.Generate(CancellationToken.None);

      // Assert
      sb.Length.Should().Be(0);
   }

   [Fact]
   public void Should_use_custom_factory_type_when_available_for_messagepack()
   {
      // Arrange
      var state = new KeyedMessagePackCodeGeneratorStateBuilder()
                  .WithIntKeyMember() // Has key member
                  .WithCustomFactory("global::MyNamespace.MyCustomType", useForSerialization: global::Thinktecture.CodeAnalysis.SerializationFrameworks.MessagePack)
                  .Build();

      var sb = new StringBuilder();
      var generator = new KeyedMessagePackCodeGenerator(state, sb);

      // Act
      generator.Generate(CancellationToken.None);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("MyNamespace.MyCustomType");
      result.Should().NotContain("System.Int32"); // Should not use key member type
   }

   [Fact]
   public void Should_use_key_member_when_custom_factory_exists_but_not_for_messagepack()
   {
      // Arrange
      var state = new KeyedMessagePackCodeGeneratorStateBuilder()
                  .WithIntKeyMember()
                  .WithCustomFactory("global::MyNamespace.MyCustomType", useForSerialization: global::Thinktecture.CodeAnalysis.SerializationFrameworks.SystemTextJson) // Not for MessagePack
                  .Build();

      var sb = new StringBuilder();
      var generator = new KeyedMessagePackCodeGenerator(state, sb);

      // Act
      generator.Generate(CancellationToken.None);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("global::System.Int32"); // Should use key member type
   }

   [Fact]
   public void Should_use_ThinktectureMessagePackFormatter_for_reference_type()
   {
      // Arrange
      var state = new KeyedMessagePackCodeGeneratorStateBuilder()
                  .AsClass() // Reference type
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();
      var generator = new KeyedMessagePackCodeGenerator(state, sb);

      // Act
      generator.Generate(CancellationToken.None);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("ThinktectureMessagePackFormatter<");
      result.Should().NotContain("ThinktectureStructMessagePackFormatter");
   }

   [Fact]
   public void Should_use_ThinktectureStructMessagePackFormatter_for_value_type()
   {
      // Arrange
      var state = new KeyedMessagePackCodeGeneratorStateBuilder()
                  .AsStruct() // Value type
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();
      var generator = new KeyedMessagePackCodeGenerator(state, sb);

      // Act
      generator.Generate(CancellationToken.None);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("ThinktectureStructMessagePackFormatter<");
      result.Should().NotContain("ThinktectureMessagePackFormatter<");
   }

   [Fact]
   public void Should_generate_with_string_key_from_key_member()
   {
      // Arrange
      var state = new KeyedMessagePackCodeGeneratorStateBuilder()
                  .WithStringKeyMember()
                  .Build();

      var sb = new StringBuilder();
      var generator = new KeyedMessagePackCodeGenerator(state, sb);

      // Act
      generator.Generate(CancellationToken.None);

      // Assert
      var result = sb.ToString();
      // MessagePack always uses 3 parameters: TType, TKey, and TValidationError
      result.Should().Contain("ThinktectureMessagePackFormatter<global::Thinktecture.Tests.TestType, global::System.String, global::Thinktecture.ValidationError>");
   }

   [Fact]
   public void Should_generate_with_string_custom_factory()
   {
      // Arrange
      var state = new KeyedMessagePackCodeGeneratorStateBuilder()
                  .WithoutKeyMember()
                  .WithStringCustomFactory()
                  .Build();

      var sb = new StringBuilder();
      var generator = new KeyedMessagePackCodeGenerator(state, sb);

      // Act
      generator.Generate(CancellationToken.None);

      // Assert
      var result = sb.ToString();
      // MessagePack always uses 3 parameters
      result.Should().Contain("ThinktectureMessagePackFormatter<global::Thinktecture.Tests.TestType, global::System.String, global::Thinktecture.ValidationError>");
   }

   [Fact]
   public void Should_generate_with_int_key()
   {
      // Arrange
      var state = new KeyedMessagePackCodeGeneratorStateBuilder()
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();
      var generator = new KeyedMessagePackCodeGenerator(state, sb);

      // Act
      generator.Generate(CancellationToken.None);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("ThinktectureMessagePackFormatter<global::Thinktecture.Tests.TestType, global::System.Int32, global::Thinktecture.ValidationError>");
   }

   [Fact]
   public void Should_generate_with_guid_key()
   {
      // Arrange
      var state = new KeyedMessagePackCodeGeneratorStateBuilder()
                  .WithGuidKeyMember()
                  .Build();

      var sb = new StringBuilder();
      var generator = new KeyedMessagePackCodeGenerator(state, sb);

      // Act
      generator.Generate(CancellationToken.None);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("ThinktectureMessagePackFormatter<global::Thinktecture.Tests.TestType, global::System.Guid, global::Thinktecture.ValidationError>");
   }

   [Fact]
   public void Should_generate_with_decimal_key()
   {
      // Arrange
      var state = new KeyedMessagePackCodeGeneratorStateBuilder()
                  .WithDecimalKeyMember()
                  .Build();

      var sb = new StringBuilder();
      var generator = new KeyedMessagePackCodeGenerator(state, sb);

      // Act
      generator.Generate(CancellationToken.None);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("ThinktectureMessagePackFormatter<global::Thinktecture.Tests.TestType, global::System.Decimal, global::Thinktecture.ValidationError>");
   }

   [Fact]
   public void Should_generate_with_byte_key()
   {
      // Arrange
      var state = new KeyedMessagePackCodeGeneratorStateBuilder()
                  .WithByteKeyMember()
                  .Build();

      var sb = new StringBuilder();
      var generator = new KeyedMessagePackCodeGenerator(state, sb);

      // Act
      generator.Generate(CancellationToken.None);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("ThinktectureMessagePackFormatter<global::Thinktecture.Tests.TestType, global::System.Byte, global::Thinktecture.ValidationError>");
   }

   [Fact]
   public void Should_omit_namespace_when_type_has_no_namespace()
   {
      // Arrange
      var state = new KeyedMessagePackCodeGeneratorStateBuilder()
                  .WithTypeWithoutNamespace()
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();
      var generator = new KeyedMessagePackCodeGenerator(state, sb);

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
      var state = new KeyedMessagePackCodeGeneratorStateBuilder()
                  .WithType("global::Thinktecture.Tests.TestType")
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();
      var generator = new KeyedMessagePackCodeGenerator(state, sb);

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
      var state = new KeyedMessagePackCodeGeneratorStateBuilder()
                  .WithNestedType(
                     typeFullyQualified: "global::Thinktecture.Tests.OuterClass.TestType",
                     name: "TestType",
                     @namespace: "Thinktecture.Tests",
                     containingType)
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();
      var generator = new KeyedMessagePackCodeGenerator(state, sb);

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
      var state = new KeyedMessagePackCodeGeneratorStateBuilder()
                  .WithNestedType(
                     typeFullyQualified: "global::Thinktecture.Tests.OuterClass.MiddleClass.TestType",
                     name: "TestType",
                     @namespace: "Thinktecture.Tests",
                     outerType, middleType)
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();
      var generator = new KeyedMessagePackCodeGenerator(state, sb);

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
      var state = new KeyedMessagePackCodeGeneratorStateBuilder()
                  .WithIntKeyMember()
                  .WithValidationError("global::MyNamespace.MyCustomValidationError")
                  .Build();

      var sb = new StringBuilder();
      var generator = new KeyedMessagePackCodeGenerator(state, sb);

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
      var state = new KeyedMessagePackCodeGeneratorStateBuilder()
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();
      var generator = new KeyedMessagePackCodeGenerator(state, sb);

      // Act
      generator.Generate(CancellationToken.None);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("// <auto-generated />");
      result.Should().Contain("#nullable enable");
   }

   [Fact]
   public void Should_apply_MessagePackFormatterAttribute()
   {
      // Arrange
      var state = new KeyedMessagePackCodeGeneratorStateBuilder()
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();
      var generator = new KeyedMessagePackCodeGenerator(state, sb);

      // Act
      generator.Generate(CancellationToken.None);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("[global::MessagePack.MessagePackFormatter(typeof(");
      result.Should().Contain("Thinktecture.Formatters.ThinktectureMessagePackFormatter");
   }

   [Fact]
   public void Should_generate_empty_partial_class_body()
   {
      // Arrange
      var state = new KeyedMessagePackCodeGeneratorStateBuilder()
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();
      var generator = new KeyedMessagePackCodeGenerator(state, sb);

      // Act
      generator.Generate(CancellationToken.None);

      // Assert
      var result = sb.ToString();
      // The partial class should be empty with just opening and closing braces
      result.Should().MatchRegex(@"partial\s+class\s+TestType\s*\{\s*\}");
   }

   [Fact]
   public void Should_handle_struct_with_nested_type()
   {
      // Arrange
      var containingType = new ContainingTypeState("Container", true, false, default);
      var state = new KeyedMessagePackCodeGeneratorStateBuilder()
                  .AsStruct()
                  .WithNestedType(
                     typeFullyQualified: "global::Thinktecture.Tests.Container.ValueStruct",
                     name: "ValueStruct",
                     @namespace: "Thinktecture.Tests",
                     containingType)
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();
      var generator = new KeyedMessagePackCodeGenerator(state, sb);

      // Act
      generator.Generate(CancellationToken.None);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("ThinktectureStructMessagePackFormatter");
      result.Should().Contain("partial class Container");
      result.Should().Contain("partial struct ValueStruct");
   }

   [Fact]
   public void Should_generate_correct_type_kind_for_struct()
   {
      // Arrange
      var state = new KeyedMessagePackCodeGeneratorStateBuilder()
                  .AsStruct()
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();
      var generator = new KeyedMessagePackCodeGenerator(state, sb);

      // Act
      generator.Generate(CancellationToken.None);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("partial struct TestType");
      result.Should().NotContain("partial class TestType");
   }

   [Fact]
   public void Should_generate_correct_type_kind_for_record()
   {
      // Arrange
      var state = new KeyedMessagePackCodeGeneratorStateBuilder()
                  .AsRecord()
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();
      var generator = new KeyedMessagePackCodeGenerator(state, sb);

      // Act
      generator.Generate(CancellationToken.None);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("partial record TestType");
   }

   // ===== Snapshot Tests =====

   [Fact]
   public async Task Snapshot_SimpleSmartEnumWithIntKey()
   {
      // Arrange
      var state = new KeyedMessagePackCodeGeneratorStateBuilder()
                  .WithType("global::Thinktecture.Tests.ProductStatus", "ProductStatus")
                  .WithIntKeyMember("Value")
                  .Build();

      var sb = new StringBuilder();
      var generator = new KeyedMessagePackCodeGenerator(state, sb);

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
      var state = new KeyedMessagePackCodeGeneratorStateBuilder()
                  .WithType("global::Thinktecture.Tests.ShippingMethod", "ShippingMethod")
                  .WithStringKeyMember("Code")
                  .Build();

      var sb = new StringBuilder();
      var generator = new KeyedMessagePackCodeGenerator(state, sb);

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
      var state = new KeyedMessagePackCodeGeneratorStateBuilder()
                  .WithType("global::Thinktecture.Tests.ProductId", "ProductId")
                  .WithGuidKeyMember("Value")
                  .Build();

      var sb = new StringBuilder();
      var generator = new KeyedMessagePackCodeGenerator(state, sb);

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
      var state = new KeyedMessagePackCodeGeneratorStateBuilder()
                  .WithType("global::Thinktecture.Tests.Amount", "Amount")
                  .WithDecimalKeyMember("Value")
                  .Build();

      var sb = new StringBuilder();
      var generator = new KeyedMessagePackCodeGenerator(state, sb);

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
      var state = new KeyedMessagePackCodeGeneratorStateBuilder()
                  .WithNestedType(
                     typeFullyQualified: "global::Thinktecture.Tests.ProductModule.ProductStatus",
                     name: "ProductStatus",
                     @namespace: "Thinktecture.Tests",
                     containingType)
                  .WithIntKeyMember("Value")
                  .Build();

      var sb = new StringBuilder();
      var generator = new KeyedMessagePackCodeGenerator(state, sb);

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
      var state = new KeyedMessagePackCodeGeneratorStateBuilder()
                  .WithNestedType(
                     typeFullyQualified: "global::Thinktecture.Tests.Level1.Level2.Level3.DeepType",
                     name: "DeepType",
                     @namespace: "Thinktecture.Tests",
                     level1, level2, level3)
                  .WithIntKeyMember("Value")
                  .Build();

      var sb = new StringBuilder();
      var generator = new KeyedMessagePackCodeGenerator(state, sb);

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
      var state = new KeyedMessagePackCodeGeneratorStateBuilder()
                  .WithType("global::Thinktecture.Tests.ISBN", "ISBN")
                  .WithoutKeyMember()
                  .WithStringCustomFactory()
                  .Build();

      var sb = new StringBuilder();
      var generator = new KeyedMessagePackCodeGenerator(state, sb);

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
      var state = new KeyedMessagePackCodeGeneratorStateBuilder()
                  .WithType("global::Thinktecture.Tests.SpecialValue", "SpecialValue")
                  .WithoutKeyMember()
                  .WithCustomFactory("global::MyNamespace.CustomKeyType")
                  .Build();

      var sb = new StringBuilder();
      var generator = new KeyedMessagePackCodeGenerator(state, sb);

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
      var state = new KeyedMessagePackCodeGeneratorStateBuilder()
                  .WithTypeWithoutNamespace("global::GlobalType", "GlobalType")
                  .WithIntKeyMember("Value")
                  .Build();

      var sb = new StringBuilder();
      var generator = new KeyedMessagePackCodeGenerator(state, sb);

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
      var state = new KeyedMessagePackCodeGeneratorStateBuilder()
                  .WithType("global::Thinktecture.Tests.ValidatedValue", "ValidatedValue")
                  .WithIntKeyMember("Value")
                  .WithValidationError("global::MyApp.Validation.CustomValidationError")
                  .Build();

      var sb = new StringBuilder();
      var generator = new KeyedMessagePackCodeGenerator(state, sb);

      // Act
      generator.Generate(CancellationToken.None);

      // Assert
      var result = sb.ToString();
      await Verifier.Verify(result).UseMethodName("Snapshot_CustomValidationErrorType");
   }

   [Fact]
   public async Task Snapshot_StructWithByteKey()
   {
      // Arrange
      var state = new KeyedMessagePackCodeGeneratorStateBuilder()
                  .AsStruct()
                  .WithType("global::Thinktecture.Tests.StatusCode", "StatusCode")
                  .WithByteKeyMember("Code")
                  .Build();

      var sb = new StringBuilder();
      var generator = new KeyedMessagePackCodeGenerator(state, sb);

      // Act
      generator.Generate(CancellationToken.None);

      // Assert
      var result = sb.ToString();
      await Verifier.Verify(result).UseMethodName("Snapshot_StructWithByteKey");
   }

   [Fact]
   public async Task Snapshot_RecordWithStringKey()
   {
      // Arrange
      var state = new KeyedMessagePackCodeGeneratorStateBuilder()
                  .AsRecord()
                  .WithType("global::Thinktecture.Tests.TagName", "TagName")
                  .WithStringKeyMember("Value")
                  .Build();

      var sb = new StringBuilder();
      var generator = new KeyedMessagePackCodeGenerator(state, sb);

      // Act
      generator.Generate(CancellationToken.None);

      // Assert
      var result = sb.ToString();
      await Verifier.Verify(result).UseMethodName("Snapshot_RecordWithStringKey");
   }
}

using System.Linq;
using Microsoft.CodeAnalysis;
using Thinktecture.CodeAnalysis;

namespace Thinktecture.Runtime.Tests.CodeAnalysis.InstanceMemberInfoTests;

// ReSharper disable once InconsistentNaming
public class CreateOrNull_Field : CompilationTestBase
{
   [Fact]
   public void Should_return_null_when_field_type_is_error()
   {
      // Arrange
      var src = @"
namespace Test;

public class TestClass
{
   private readonly NonExistentType _field;
}
";
      var compilation = CreateCompilation(src, expectedCompilerErrors: ["The type or namespace name 'NonExistentType' could not be found (are you missing a using directive or an assembly reference?)"]);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var field = (IFieldSymbol)type.GetMembers("_field").First();

      // Act
      var result = InstanceMemberInfo.CreateOrNull(factory, field, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

      // Assert
      result.Should().BeNull();
   }

   [Fact]
   public void Should_create_instance_for_valid_field()
   {
      // Arrange
      var src = @"
namespace Test;

public class TestClass
{
   private readonly int _field;
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var field = (IFieldSymbol)type.GetMembers("_field").First();

      // Act
      var result = InstanceMemberInfo.CreateOrNull(factory, field, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

      // Assert
      result.Should().NotBeNull();
      result!.Name.Should().Be("_field");
      result.Kind.Should().Be(SymbolKind.Field);
      result.IsStatic.Should().BeFalse();
      result.IsErroneous.Should().BeFalse();
      result.IsAbstract.Should().BeFalse();
      result.SpecialType.Should().Be(SpecialType.System_Int32);
   }

   [Fact]
   public void Should_set_IsStatic_to_true_for_static_field()
   {
      // Arrange
      var src = @"
namespace Test;

public class TestClass
{
   public static readonly string StaticField = ""test"";
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var field = (IFieldSymbol)type.GetMembers("StaticField").First();

      // Act
      var result = InstanceMemberInfo.CreateOrNull(factory, field, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

      // Assert
      result.Should().NotBeNull();
      result!.IsStatic.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_nullable_reference_type_field()
   {
      // Arrange
      var src = @"
#nullable enable
namespace Test;

public class TestClass
{
   private readonly string? _field;
}
";
      var compilation = CreateCompilation(src, nullableContextOptions: NullableContextOptions.Enable);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var field = (IFieldSymbol)type.GetMembers("_field").First();

      // Act
      var result = InstanceMemberInfo.CreateOrNull(factory, field, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

      // Assert
      result.Should().NotBeNull();
      result!.NullableAnnotation.Should().Be(NullableAnnotation.Annotated);
      result.IsReferenceType.Should().BeTrue();
      result.IsReferenceTypeOrNullableStruct.Should().BeTrue();
      result.IsNullableStruct.Should().BeFalse();
   }

   [Fact]
   public void Should_handle_nullable_value_type_field()
   {
      // Arrange
      var src = @"
namespace Test;

public class TestClass
{
   private readonly int? _field;
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var field = (IFieldSymbol)type.GetMembers("_field").First();

      // Act
      var result = InstanceMemberInfo.CreateOrNull(factory, field, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

      // Assert
      result.Should().NotBeNull();
      result!.IsNullableStruct.Should().BeTrue();
      result.IsReferenceTypeOrNullableStruct.Should().BeTrue();
      result.IsValueType.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_field_with_MessagePackKey_attribute()
   {
      // Arrange
      var src = @"
using MessagePack;

namespace Test;

public class TestClass
{
   [Key(5)]
   private readonly int _field;
}
";
      var compilation = CreateCompilation(src, additionalReferences: [typeof(MessagePack.MessagePackObjectAttribute).Assembly.Location]);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var field = (IFieldSymbol)type.GetMembers("_field").First();

      // Act
      var result = InstanceMemberInfo.CreateOrNull(factory, field, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

      // Assert
      result.Should().NotBeNull();
      result!.MessagePackKey.Should().Be(5);
   }

   [Fact]
   public void Should_return_null_MessagePackKey_when_no_attribute()
   {
      // Arrange
      var src = @"
namespace Test;

public class TestClass
{
   private readonly int _field;
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var field = (IFieldSymbol)type.GetMembers("_field").First();

      // Act
      var result = InstanceMemberInfo.CreateOrNull(factory, field, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

      // Assert
      result.Should().NotBeNull();
      result!.MessagePackKey.Should().BeNull();
   }

   [Fact]
   public void Should_handle_field_with_JsonIgnore_attribute()
   {
      // Arrange
      var src = @"
using System.Text.Json.Serialization;

namespace Test;

public class TestClass
{
   [JsonIgnore]
   private readonly int _field;
}
";
      var compilation = CreateCompilation(src, additionalReferences: [typeof(System.Text.Json.Serialization.JsonConverterAttribute).Assembly.Location]);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var field = (IFieldSymbol)type.GetMembers("_field").First();

      // Act
      var result = InstanceMemberInfo.CreateOrNull(factory, field, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

      // Assert
      result.Should().NotBeNull();
      result!.JsonIgnoreCondition.Should().Be(Json.JsonIgnoreCondition.Always);
   }

   [Fact]
   public void Should_handle_field_with_JsonIgnore_attribute_with_condition()
   {
      // Arrange
      var src = @"
using System.Text.Json.Serialization;

namespace Test;

public class TestClass
{
   [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
   private readonly string _field;
}
";
      var compilation = CreateCompilation(src, additionalReferences: [typeof(System.Text.Json.Serialization.JsonConverterAttribute).Assembly.Location]);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var field = (IFieldSymbol)type.GetMembers("_field").First();

      // Act
      var result = InstanceMemberInfo.CreateOrNull(factory, field, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

      // Assert
      result.Should().NotBeNull();
      result!.JsonIgnoreCondition.Should().Be(Json.JsonIgnoreCondition.WhenWritingNull);
   }

   [Fact]
   public void Should_return_null_JsonIgnoreCondition_when_no_attribute()
   {
      // Arrange
      var src = @"
namespace Test;

public class TestClass
{
   private readonly int _field;
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var field = (IFieldSymbol)type.GetMembers("_field").First();

      // Act
      var result = InstanceMemberInfo.CreateOrNull(factory, field, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

      // Assert
      result.Should().NotBeNull();
      result!.JsonIgnoreCondition.Should().BeNull();
   }

   [Fact]
   public void Should_populate_ValueObjectMemberSettings_when_parameter_is_true()
   {
      // Arrange
      var src = @"
namespace Test;

public class TestClass
{
   [Thinktecture.MemberEqualityComparer<Thinktecture.ComparerAccessors.StringOrdinalIgnoreCase, string>]
   private readonly int _field;
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var field = (IFieldSymbol)type.GetMembers("_field").First();

      // Act
      var result = InstanceMemberInfo.CreateOrNull(factory, field, populateValueObjectMemberSettings: true, allowedCaptureSymbols: false);

      // Assert
      result.Should().NotBeNull();
      result!.ValueObjectMemberSettings.Should().NotBe(Thinktecture.CodeAnalysis.ValueObjects.ValueObjectMemberSettings.None);
   }

   [Fact]
   public void Should_not_populate_ValueObjectMemberSettings_when_parameter_is_false()
   {
      // Arrange
      var src = @"
namespace Test;

public class TestClass
{
   private readonly int _field;
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var field = (IFieldSymbol)type.GetMembers("_field").First();

      // Act
      var result = InstanceMemberInfo.CreateOrNull(factory, field, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

      // Assert
      result.Should().NotBeNull();
      result!.ValueObjectMemberSettings.Should().Be(Thinktecture.CodeAnalysis.ValueObjects.ValueObjectMemberSettings.None);
   }

   [Fact]
   public void Should_handle_field_of_value_object_type_that_disallows_default()
   {
      // Arrange
      var src = @"
using Thinktecture;

namespace Test;

[ValueObject<int>]
public partial struct MyValueObject
{
}

public class TestClass
{
   private readonly MyValueObject _field;
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var field = (IFieldSymbol)type.GetMembers("_field").First();

      // Act
      var result = InstanceMemberInfo.CreateOrNull(factory, field, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

      // Assert
      result.Should().NotBeNull();
      result!.DisallowsDefaultValue.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_field_of_value_object_type_that_allows_default()
   {
      // Arrange
      var src = @"
using Thinktecture;

namespace Test;

[ValueObject<int>(AllowDefaultStructs = true)]
public partial struct MyValueObject
{
}

public class TestClass
{
   private readonly MyValueObject _field;
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var field = (IFieldSymbol)type.GetMembers("_field").First();

      // Act
      var result = InstanceMemberInfo.CreateOrNull(factory, field, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

      // Assert
      result.Should().NotBeNull();
      result!.DisallowsDefaultValue.Should().BeFalse();
   }

   [Fact]
   public void Should_disallow_default_for_keyed_value_object_with_reference_type_key()
   {
      // Arrange
      var src = @"
using Thinktecture;

namespace Test;

[ValueObject<string>(AllowDefaultStructs = true)]
public partial struct MyValueObject
{
}

public class TestClass
{
   private readonly MyValueObject _field;
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var field = (IFieldSymbol)type.GetMembers("_field").First();

      // Act
      var result = InstanceMemberInfo.CreateOrNull(factory, field, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

      // Assert
      result.Should().NotBeNull();
      result!.DisallowsDefaultValue.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_field_implementing_IDisallowDefaultValue()
   {
      // Arrange
      var src = @"
using Thinktecture;

namespace Test;

public struct MyStruct : IDisallowDefaultValue
{
}

public class TestClass
{
   private readonly MyStruct _field;
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var field = (IFieldSymbol)type.GetMembers("_field").First();

      // Act
      var result = InstanceMemberInfo.CreateOrNull(factory, field, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

      // Assert
      result.Should().NotBeNull();
      result!.DisallowsDefaultValue.Should().BeTrue();
   }

   [Fact]
   public void Should_not_disallow_default_for_primitive_types()
   {
      // Arrange
      var src = @"
namespace Test;

public class TestClass
{
   private readonly int _field;
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var field = (IFieldSymbol)type.GetMembers("_field").First();

      // Act
      var result = InstanceMemberInfo.CreateOrNull(factory, field, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

      // Assert
      result.Should().NotBeNull();
      result!.DisallowsDefaultValue.Should().BeFalse();
   }

   [Fact]
   public void Should_not_disallow_default_for_reference_types()
   {
      // Arrange
      var src = @"
namespace Test;

public class TestClass
{
   private readonly string _field;
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var field = (IFieldSymbol)type.GetMembers("_field").First();

      // Act
      var result = InstanceMemberInfo.CreateOrNull(factory, field, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

      // Assert
      result.Should().NotBeNull();
      result!.DisallowsDefaultValue.Should().BeFalse();
   }

   [Fact]
   public void Should_handle_field_with_special_type()
   {
      // Arrange
      var src = @"
namespace Test;

public class TestClass
{
   private readonly string _stringField;
   private readonly int _intField;
   private readonly bool _boolField;
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");

      // Act
      var stringResult = InstanceMemberInfo.CreateOrNull(factory, (IFieldSymbol)type.GetMembers("_stringField").First(), false, false);
      var intResult = InstanceMemberInfo.CreateOrNull(factory, (IFieldSymbol)type.GetMembers("_intField").First(), false, false);
      var boolResult = InstanceMemberInfo.CreateOrNull(factory, (IFieldSymbol)type.GetMembers("_boolField").First(), false, false);

      // Assert
      stringResult!.SpecialType.Should().Be(SpecialType.System_String);
      intResult!.SpecialType.Should().Be(SpecialType.System_Int32);
      boolResult!.SpecialType.Should().Be(SpecialType.System_Boolean);
   }

   [Fact]
   public void Should_initialize_ArgumentName_correctly()
   {
      // Arrange
      var src = @"
namespace Test;

public class TestClass
{
   private readonly int _myField;
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var field = (IFieldSymbol)type.GetMembers("_myField").First();

      // Act
      var result = InstanceMemberInfo.CreateOrNull(factory, field, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

      // Assert
      result.Should().NotBeNull();
      result!.ArgumentName.Name.Should().Be("_myField");
   }

   [Fact]
   public void Should_set_IsRecord_to_false()
   {
      // Arrange
      var src = @"
namespace Test;

public class TestClass
{
   private readonly int _field;
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var field = (IFieldSymbol)type.GetMembers("_field").First();

      // Act
      var result = InstanceMemberInfo.CreateOrNull(factory, field, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

      // Assert
      result.Should().NotBeNull();
      result!.IsRecord.Should().BeFalse();
   }

   [Fact]
   public void Should_handle_MessagePackKey_with_non_integer_type()
   {
      // Arrange
      var src = @"
using MessagePack;

namespace Test;

public class TestClass
{
   [Key(""notAnInt"")]
   private readonly int _field;
}
";
      var compilation = CreateCompilation(src, additionalReferences: [typeof(MessagePack.KeyAttribute).Assembly.Location]);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");

      if (type.GetMembers("_field").FirstOrDefault() is IFieldSymbol field)
      {
         // Act
         var result = InstanceMemberInfo.CreateOrNull(factory, field, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

         // Assert
         result?.MessagePackKey.Should().BeNull();
      }
   }

   [Fact]
   public void Should_handle_MessagePackKey_with_zero()
   {
      // Arrange
      var src = @"
using MessagePack;

namespace Test;

public class TestClass
{
   [Key(0)]
   private readonly int _field;
}
";
      var compilation = CreateCompilation(src, additionalReferences: [typeof(MessagePack.KeyAttribute).Assembly.Location]);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var field = (IFieldSymbol)type.GetMembers("_field").First();

      // Act
      var result = InstanceMemberInfo.CreateOrNull(factory, field, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

      // Assert
      result.Should().NotBeNull();
      result!.MessagePackKey.Should().Be(0);
   }

   [Fact]
   public void Should_handle_MessagePackKey_with_negative_value()
   {
      // Arrange
      var src = @"
using MessagePack;

namespace Test;

public class TestClass
{
   [Key(-1)]
   private readonly int _field;
}
";
      var compilation = CreateCompilation(src, additionalReferences: [typeof(MessagePack.MessagePackObjectAttribute).Assembly.Location]);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var field = (IFieldSymbol)type.GetMembers("_field").First();

      // Act
      var result = InstanceMemberInfo.CreateOrNull(factory, field, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

      // Assert
      result.Should().NotBeNull();
      result!.MessagePackKey.Should().Be(-1);
   }
}

using System.Linq;
using Microsoft.CodeAnalysis;
using Thinktecture.CodeAnalysis;

namespace Thinktecture.Runtime.Tests.CodeAnalysis.InstanceMemberInfoTests;

public class CreateOrNull_Property : CompilationTestBase
{
   [Fact]
   public void Should_return_null_when_property_type_is_error()
   {
      // Arrange
      var src = @"
namespace Test;

public class TestClass
{
   public NonExistentType Property { get; }
}
";
      var compilation = CreateCompilation(src, expectedCompilerErrors: ["The type or namespace name 'NonExistentType' could not be found (are you missing a using directive or an assembly reference?)"]);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var property = (IPropertySymbol)type.GetMembers("Property").First();

      // Act
      var result = InstanceMemberInfo.CreateOrNull(factory, property, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

      // Assert
      result.Should().BeNull();
   }

   [Fact]
   public void Should_create_instance_for_valid_property()
   {
      // Arrange
      var src = @"
namespace Test;

public class TestClass
{
   public int Property { get; }
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var property = (IPropertySymbol)type.GetMembers("Property").First();

      // Act
      var result = InstanceMemberInfo.CreateOrNull(factory, property, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

      // Assert
      result.Should().NotBeNull();
      result!.Name.Should().Be("Property");
      result.Kind.Should().Be(SymbolKind.Property);
      result.IsStatic.Should().BeFalse();
      result.IsErroneous.Should().BeFalse();
      result.IsAbstract.Should().BeFalse();
      result.SpecialType.Should().Be(SpecialType.System_Int32);
   }

   [Fact]
   public void Should_set_IsStatic_to_true_for_static_property()
   {
      // Arrange
      var src = @"
namespace Test;

public class TestClass
{
   public static string StaticProperty { get; } = ""test"";
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var property = (IPropertySymbol)type.GetMembers("StaticProperty").First();

      // Act
      var result = InstanceMemberInfo.CreateOrNull(factory, property, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

      // Assert
      result.Should().NotBeNull();
      result!.IsStatic.Should().BeTrue();
   }

   [Fact]
   public void Should_set_IsAbstract_to_true_for_abstract_property()
   {
      // Arrange
      var src = @"
namespace Test;

public abstract class TestClass
{
   public abstract int AbstractProperty { get; }
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var property = (IPropertySymbol)type.GetMembers("AbstractProperty").First();

      // Act
      var result = InstanceMemberInfo.CreateOrNull(factory, property, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

      // Assert
      result.Should().NotBeNull();
      result!.IsAbstract.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_nullable_reference_type_property()
   {
      // Arrange
      var src = @"
#nullable enable
namespace Test;

public class TestClass
{
   public string? Property { get; }
}
";
      var compilation = CreateCompilation(src, nullableContextOptions: NullableContextOptions.Enable);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var property = (IPropertySymbol)type.GetMembers("Property").First();

      // Act
      var result = InstanceMemberInfo.CreateOrNull(factory, property, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

      // Assert
      result.Should().NotBeNull();
      result!.NullableAnnotation.Should().Be(NullableAnnotation.Annotated);
      result.IsReferenceType.Should().BeTrue();
      result.IsReferenceTypeOrNullableStruct.Should().BeTrue();
      result.IsNullableStruct.Should().BeFalse();
   }

   [Fact]
   public void Should_handle_nullable_value_type_property()
   {
      // Arrange
      var src = @"
namespace Test;

public class TestClass
{
   public int? Property { get; }
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var property = (IPropertySymbol)type.GetMembers("Property").First();

      // Act
      var result = InstanceMemberInfo.CreateOrNull(factory, property, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

      // Assert
      result.Should().NotBeNull();
      result!.IsNullableStruct.Should().BeTrue();
      result.IsReferenceTypeOrNullableStruct.Should().BeTrue();
      result.IsValueType.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_property_with_MessagePackKey_attribute()
   {
      // Arrange
      var src = @"
using MessagePack;

namespace Test;

public class TestClass
{
   [Key(10)]
   public int Property { get; }
}
";
      var compilation = CreateCompilation(src, additionalReferences: [typeof(MessagePack.KeyAttribute).Assembly.Location]);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var property = (IPropertySymbol)type.GetMembers("Property").First();

      // Act
      var result = InstanceMemberInfo.CreateOrNull(factory, property, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

      // Assert
      result.Should().NotBeNull();
      result!.MessagePackKey.Should().Be(10);
   }

   [Fact]
   public void Should_return_null_MessagePackKey_when_no_attribute()
   {
      // Arrange
      var src = @"
namespace Test;

public class TestClass
{
   public int Property { get; }
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var property = (IPropertySymbol)type.GetMembers("Property").First();

      // Act
      var result = InstanceMemberInfo.CreateOrNull(factory, property, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

      // Assert
      result.Should().NotBeNull();
      result!.MessagePackKey.Should().BeNull();
   }

   [Fact]
   public void Should_handle_property_with_JsonIgnore_attribute()
   {
      // Arrange
      var src = @"
using System.Text.Json.Serialization;

namespace Test;

public class TestClass
{
   [JsonIgnore]
   public int Property { get; }
}
";
      var compilation = CreateCompilation(src, additionalReferences: [typeof(System.Text.Json.Serialization.JsonConverterAttribute).Assembly.Location]);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var property = (IPropertySymbol)type.GetMembers("Property").First();

      // Act
      var result = InstanceMemberInfo.CreateOrNull(factory, property, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

      // Assert
      result.Should().NotBeNull();
      result!.JsonIgnoreCondition.Should().Be(Json.JsonIgnoreCondition.Always);
   }

   [Fact]
   public void Should_handle_property_with_JsonIgnore_attribute_with_condition()
   {
      // Arrange
      var src = @"
using System.Text.Json.Serialization;

namespace Test;

public class TestClass
{
   [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
   public string Property { get; }
}
";
      var compilation = CreateCompilation(src, additionalReferences: [typeof(System.Text.Json.Serialization.JsonConverterAttribute).Assembly.Location]);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var property = (IPropertySymbol)type.GetMembers("Property").First();

      // Act
      var result = InstanceMemberInfo.CreateOrNull(factory, property, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

      // Assert
      result.Should().NotBeNull();
      result!.JsonIgnoreCondition.Should().Be(Json.JsonIgnoreCondition.WhenWritingDefault);
   }

   [Fact]
   public void Should_return_null_JsonIgnoreCondition_when_no_attribute()
   {
      // Arrange
      var src = @"
namespace Test;

public class TestClass
{
   public int Property { get; }
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var property = (IPropertySymbol)type.GetMembers("Property").First();

      // Act
      var result = InstanceMemberInfo.CreateOrNull(factory, property, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

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
   public int Property { get; }
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var property = (IPropertySymbol)type.GetMembers("Property").First();

      // Act
      var result = InstanceMemberInfo.CreateOrNull(factory, property, populateValueObjectMemberSettings: true, allowedCaptureSymbols: false);

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
   public int Property { get; }
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var property = (IPropertySymbol)type.GetMembers("Property").First();

      // Act
      var result = InstanceMemberInfo.CreateOrNull(factory, property, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

      // Assert
      result.Should().NotBeNull();
      result!.ValueObjectMemberSettings.Should().Be(Thinktecture.CodeAnalysis.ValueObjects.ValueObjectMemberSettings.None);
   }

   [Fact]
   public void Should_handle_property_of_value_object_type_that_disallows_default()
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
   public MyValueObject Property { get; }
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var property = (IPropertySymbol)type.GetMembers("Property").First();

      // Act
      var result = InstanceMemberInfo.CreateOrNull(factory, property, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

      // Assert
      result.Should().NotBeNull();
      result!.DisallowsDefaultValue.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_property_of_value_object_type_that_allows_default()
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
   public MyValueObject Property { get; }
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var property = (IPropertySymbol)type.GetMembers("Property").First();

      // Act
      var result = InstanceMemberInfo.CreateOrNull(factory, property, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

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
   public MyValueObject Property { get; }
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var property = (IPropertySymbol)type.GetMembers("Property").First();

      // Act
      var result = InstanceMemberInfo.CreateOrNull(factory, property, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

      // Assert
      result.Should().NotBeNull();
      result!.DisallowsDefaultValue.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_property_implementing_IDisallowDefaultValue()
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
   public MyStruct Property { get; }
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var property = (IPropertySymbol)type.GetMembers("Property").First();

      // Act
      var result = InstanceMemberInfo.CreateOrNull(factory, property, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

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
   public int Property { get; }
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var property = (IPropertySymbol)type.GetMembers("Property").First();

      // Act
      var result = InstanceMemberInfo.CreateOrNull(factory, property, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

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
   public string Property { get; }
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var property = (IPropertySymbol)type.GetMembers("Property").First();

      // Act
      var result = InstanceMemberInfo.CreateOrNull(factory, property, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

      // Assert
      result.Should().NotBeNull();
      result!.DisallowsDefaultValue.Should().BeFalse();
   }

   [Fact]
   public void Should_handle_property_with_special_type()
   {
      // Arrange
      var src = @"
namespace Test;

public class TestClass
{
   public string StringProperty { get; }
   public int IntProperty { get; }
   public bool BoolProperty { get; }
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");

      // Act
      var stringResult = InstanceMemberInfo.CreateOrNull(factory, (IPropertySymbol)type.GetMembers("StringProperty").First(), false, false);
      var intResult = InstanceMemberInfo.CreateOrNull(factory, (IPropertySymbol)type.GetMembers("IntProperty").First(), false, false);
      var boolResult = InstanceMemberInfo.CreateOrNull(factory, (IPropertySymbol)type.GetMembers("BoolProperty").First(), false, false);

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
   public int MyProperty { get; }
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var property = (IPropertySymbol)type.GetMembers("MyProperty").First();

      // Act
      var result = InstanceMemberInfo.CreateOrNull(factory, property, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

      // Assert
      result.Should().NotBeNull();
      result!.ArgumentName.Name.Should().Be("MyProperty");
   }

   [Fact]
   public void Should_set_IsRecord_to_false()
   {
      // Arrange
      var src = @"
namespace Test;

public class TestClass
{
   public int Property { get; }
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var property = (IPropertySymbol)type.GetMembers("Property").First();

      // Act
      var result = InstanceMemberInfo.CreateOrNull(factory, property, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

      // Assert
      result.Should().NotBeNull();
      result!.IsRecord.Should().BeFalse();
   }

   [Fact]
   public void Should_handle_property_with_private_init_accessor()
   {
      // Arrange
      var src = @"
namespace Test;

public class TestClass
{
   public string Name { get; private init; }
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var property = (IPropertySymbol)type.GetMembers("Name").First();

      // Act
      var result = InstanceMemberInfo.CreateOrNull(factory, property, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

      // Assert
      result.Should().NotBeNull();
      result!.Name.Should().Be("Name");
   }

   [Fact]
   public void Should_handle_property_with_init_accessor()
   {
      // Arrange
      var src = @"
namespace Test;

public class TestClass
{
   public string Name { get; init; }
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var property = (IPropertySymbol)type.GetMembers("Name").First();

      // Act
      var result = InstanceMemberInfo.CreateOrNull(factory, property, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

      // Assert
      result.Should().NotBeNull();
      result!.Name.Should().Be("Name");
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
   public int Property { get; }
}
";
      var compilation = CreateCompilation(src, additionalReferences: [typeof(MessagePack.KeyAttribute).Assembly.Location]);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var property = (IPropertySymbol)type.GetMembers("Property").First();

      // Act
      var result = InstanceMemberInfo.CreateOrNull(factory, property, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

      // Assert
      result.Should().NotBeNull();
      result!.MessagePackKey.Should().Be(0);
   }

   [Fact]
   public void Should_handle_MessagePackKey_with_large_value()
   {
      // Arrange
      var src = @"
using MessagePack;

namespace Test;

public class TestClass
{
   [Key(999999)]
   public int Property { get; }
}
";
      var compilation = CreateCompilation(src, additionalReferences: [typeof(MessagePack.KeyAttribute).Assembly.Location]);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var property = (IPropertySymbol)type.GetMembers("Property").First();

      // Act
      var result = InstanceMemberInfo.CreateOrNull(factory, property, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

      // Assert
      result.Should().NotBeNull();
      result!.MessagePackKey.Should().Be(999999);
   }

   [Fact]
   public void Should_handle_JsonIgnore_condition_Never()
   {
      // Arrange
      var src = @"
using System.Text.Json.Serialization;

namespace Test;

public class TestClass
{
   [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
   public string Property { get; }
}
";
      var compilation = CreateCompilation(src, additionalReferences: [typeof(System.Text.Json.Serialization.JsonConverterAttribute).Assembly.Location]);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var property = (IPropertySymbol)type.GetMembers("Property").First();

      // Act
      var result = InstanceMemberInfo.CreateOrNull(factory, property, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

      // Assert
      result.Should().NotBeNull();
      result!.JsonIgnoreCondition.Should().Be(Json.JsonIgnoreCondition.Never);
   }
}

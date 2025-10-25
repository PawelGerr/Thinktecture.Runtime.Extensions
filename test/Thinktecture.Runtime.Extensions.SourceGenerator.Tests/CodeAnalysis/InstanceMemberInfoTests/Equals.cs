using System.Linq;
using Microsoft.CodeAnalysis;
using Thinktecture.CodeAnalysis;

#nullable enable

namespace Thinktecture.Runtime.Tests.CodeAnalysis.InstanceMemberInfoTests;

public class Equals : CompilationTestBase
{
   [Fact]
   public void Should_return_true_for_same_instance()
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
      var memberInfo = InstanceMemberInfo.CreateOrNull(factory, field, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

      // Act
      var result = memberInfo!.Equals(memberInfo);

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_false_for_null()
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
      var memberInfo = InstanceMemberInfo.CreateOrNull(factory, field, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

      // Act
      var result = memberInfo!.Equals((InstanceMemberInfo?)null);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_for_non_InstanceMemberInfo_object()
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
      var memberInfo = InstanceMemberInfo.CreateOrNull(factory, field, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

      // Act
      var result = memberInfo!.Equals(new object());

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_true_for_identical_fields()
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
      var memberInfo1 = InstanceMemberInfo.CreateOrNull(factory, field, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);
      var memberInfo2 = InstanceMemberInfo.CreateOrNull(factory, field, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

      // Act
      var result = memberInfo1!.Equals(memberInfo2);

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_false_for_different_field_names()
   {
      // Arrange
      var src = @"
namespace Test;

public class TestClass
{
   private readonly int _field1;
   private readonly int _field2;
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var field1 = (IFieldSymbol)type.GetMembers("_field1").First();
      var field2 = (IFieldSymbol)type.GetMembers("_field2").First();
      var memberInfo1 = InstanceMemberInfo.CreateOrNull(factory, field1, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);
      var memberInfo2 = InstanceMemberInfo.CreateOrNull(factory, field2, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

      // Act
      var result = memberInfo1!.Equals(memberInfo2);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_for_different_types()
   {
      // Arrange
      var src = @"
namespace Test;

public class TestClass
{
   private readonly int _intField;
   private readonly string _stringField;
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var intField = (IFieldSymbol)type.GetMembers("_intField").First();
      var stringField = (IFieldSymbol)type.GetMembers("_stringField").First();
      var memberInfo1 = InstanceMemberInfo.CreateOrNull(factory, intField, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);
      var memberInfo2 = InstanceMemberInfo.CreateOrNull(factory, stringField, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

      // Act
      var result = memberInfo1!.Equals(memberInfo2);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_for_different_IsStatic_values()
   {
      // Arrange
      var src = @"
namespace Test;

public class TestClass
{
   private readonly int _instanceField;
   public static readonly int StaticField = 0;
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var instanceField = (IFieldSymbol)type.GetMembers("_instanceField").First();
      var staticField = (IFieldSymbol)type.GetMembers("StaticField").First();
      var memberInfo1 = InstanceMemberInfo.CreateOrNull(factory, instanceField, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);
      var memberInfo2 = InstanceMemberInfo.CreateOrNull(factory, staticField, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

      // Act
      var result = memberInfo1!.Equals(memberInfo2);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_for_different_Kind_values()
   {
      // Arrange
      var src = @"
namespace Test;

public class TestClass
{
   private readonly int _field;
   public int Property { get; }
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var field = (IFieldSymbol)type.GetMembers("_field").First();
      var property = (IPropertySymbol)type.GetMembers("Property").First();
      var memberInfo1 = InstanceMemberInfo.CreateOrNull(factory, field, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);
      var memberInfo2 = InstanceMemberInfo.CreateOrNull(factory, property, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

      // Act
      var result = memberInfo1!.Equals(memberInfo2);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_for_different_NullableAnnotation_values()
   {
      // Arrange
      var src = @"
#nullable enable
namespace Test;

public class TestClass
{
   private readonly string? _nullableField;
   private readonly string _nonNullableField;
}
";
      var compilation = CreateCompilation(src, nullableContextOptions: NullableContextOptions.Enable);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var nullableField = (IFieldSymbol)type.GetMembers("_nullableField").First();
      var nonNullableField = (IFieldSymbol)type.GetMembers("_nonNullableField").First();
      var memberInfo1 = InstanceMemberInfo.CreateOrNull(factory, nullableField, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);
      var memberInfo2 = InstanceMemberInfo.CreateOrNull(factory, nonNullableField, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

      // Act
      var result = memberInfo1!.Equals(memberInfo2);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_for_different_MessagePackKey_values()
   {
      // Arrange
      var src = @"
using MessagePack;

namespace Test;

public class TestClass
{
   [Key(1)]
   private readonly int _field1;

   [Key(2)]
   private readonly int _field2;
}
";
      var compilation = CreateCompilation(src, additionalReferences: [typeof(MessagePack.MessagePackFormatterAttribute).Assembly.Location]);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var field1 = (IFieldSymbol)type.GetMembers("_field1").First();
      var field2 = (IFieldSymbol)type.GetMembers("_field2").First();
      var memberInfo1 = InstanceMemberInfo.CreateOrNull(factory, field1, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);
      var memberInfo2 = InstanceMemberInfo.CreateOrNull(factory, field2, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

      // Act
      var result = memberInfo1!.Equals(memberInfo2);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_for_different_JsonIgnoreCondition_values()
   {
      // Arrange
      var src = @"
using System.Text.Json.Serialization;

namespace Test;

public class TestClass
{
   [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
   private readonly string _field1;

   [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
   private readonly string _field2;
}
";
      var compilation = CreateCompilation(src, additionalReferences: [typeof(System.Text.Json.Serialization.JsonConverterAttribute).Assembly.Location]);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var field1 = (IFieldSymbol)type.GetMembers("_field1").First();
      var field2 = (IFieldSymbol)type.GetMembers("_field2").First();
      var memberInfo1 = InstanceMemberInfo.CreateOrNull(factory, field1, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);
      var memberInfo2 = InstanceMemberInfo.CreateOrNull(factory, field2, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

      // Act
      var result = memberInfo1!.Equals(memberInfo2);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_for_different_DisallowsDefaultValue_values()
   {
      // Arrange
      var src = @"
using Thinktecture;

namespace Test;

[ValueObject<int>]
public partial struct ValueObject1
{
}

[ValueObject<int>(AllowDefaultStructs = true)]
public partial struct ValueObject2
{
}

public class TestClass
{
   private readonly ValueObject1 _field1;
   private readonly ValueObject2 _field2;
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var field1 = (IFieldSymbol)type.GetMembers("_field1").First();
      var field2 = (IFieldSymbol)type.GetMembers("_field2").First();
      var memberInfo1 = InstanceMemberInfo.CreateOrNull(factory, field1, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);
      var memberInfo2 = InstanceMemberInfo.CreateOrNull(factory, field2, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

      // Act
      var result = memberInfo1!.Equals(memberInfo2);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_for_different_ValueObjectMemberSettings()
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
      var memberInfo1 = InstanceMemberInfo.CreateOrNull(factory, field, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);
      var memberInfo2 = InstanceMemberInfo.CreateOrNull(factory, field, populateValueObjectMemberSettings: true, allowedCaptureSymbols: false);

      // Act
      var result = memberInfo1!.Equals(memberInfo2);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_true_for_equal_fields_with_same_attributes()
   {
      // Arrange
      var src = @"
using MessagePack;
using System.Text.Json.Serialization;

namespace Test;

public class TestClass
{
   [Key(5)]
   [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
   private readonly int _field;
}
";
      var compilation = CreateCompilation(src, additionalReferences: [typeof(MessagePack.MessagePackFormatterAttribute).Assembly.Location]);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var field = (IFieldSymbol)type.GetMembers("_field").First();
      var memberInfo1 = InstanceMemberInfo.CreateOrNull(factory, field, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);
      var memberInfo2 = InstanceMemberInfo.CreateOrNull(factory, field, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

      // Act
      var result = memberInfo1!.Equals(memberInfo2);

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_for_equal_properties()
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
      var memberInfo1 = InstanceMemberInfo.CreateOrNull(factory, property, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);
      var memberInfo2 = InstanceMemberInfo.CreateOrNull(factory, property, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

      // Act
      var result = memberInfo1!.Equals(memberInfo2);

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_equality_operators()
   {
      // Arrange
      var src = @"
namespace Test;

public class TestClass
{
   private readonly int _field1;
   private readonly int _field2;
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var field1 = (IFieldSymbol)type.GetMembers("_field1").First();
      var field2 = (IFieldSymbol)type.GetMembers("_field2").First();
      var memberInfo1a = InstanceMemberInfo.CreateOrNull(factory, field1, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);
      var memberInfo1b = InstanceMemberInfo.CreateOrNull(factory, field1, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);
      var memberInfo2 = InstanceMemberInfo.CreateOrNull(factory, field2, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

      // Act & Assert
      (memberInfo1a == memberInfo1b).Should().BeFalse();    // No operator overload, so reference equality
      memberInfo1a!.Equals(memberInfo1b).Should().BeTrue(); // But value equality should work
      memberInfo1a.Equals(memberInfo2).Should().BeFalse();
   }

   [Fact]
   public void Should_return_true_for_fields_with_same_MessagePackKey_null()
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
      var memberInfo1 = InstanceMemberInfo.CreateOrNull(factory, field, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);
      var memberInfo2 = InstanceMemberInfo.CreateOrNull(factory, field, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

      // Act
      var result = memberInfo1!.Equals(memberInfo2);

      // Assert
      result.Should().BeTrue();
      memberInfo1.MessagePackKey.Should().BeNull();
      memberInfo2!.MessagePackKey.Should().BeNull();
   }

   [Fact]
   public void Should_return_true_for_fields_with_same_JsonIgnoreCondition_null()
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
      var memberInfo1 = InstanceMemberInfo.CreateOrNull(factory, field, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);
      var memberInfo2 = InstanceMemberInfo.CreateOrNull(factory, field, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

      // Act
      var result = memberInfo1!.Equals(memberInfo2);

      // Assert
      result.Should().BeTrue();
      memberInfo1.JsonIgnoreCondition.Should().BeNull();
      memberInfo2!.JsonIgnoreCondition.Should().BeNull();
   }
}

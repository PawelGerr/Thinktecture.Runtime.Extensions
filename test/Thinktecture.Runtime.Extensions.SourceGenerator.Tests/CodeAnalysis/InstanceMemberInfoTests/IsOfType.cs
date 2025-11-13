using System.Linq;
using Microsoft.CodeAnalysis;
using Thinktecture.CodeAnalysis;

namespace Thinktecture.Runtime.Tests.CodeAnalysis.InstanceMemberInfoTests;

public class IsOfType : CompilationTestBase
{
   [Fact]
   public void Should_return_true_for_field_with_matching_type()
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
      var memberInfo = InstanceMemberInfo.CreateOrNull(factory, field, populateValueObjectMemberSettings: false, allowedCaptureSymbols: true);
      var intType = compilation.GetSpecialType(SpecialType.System_Int32);

      // Act
      var result = memberInfo!.IsOfType(intType);

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_false_for_field_with_non_matching_type()
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
      var memberInfo = InstanceMemberInfo.CreateOrNull(factory, field, populateValueObjectMemberSettings: false, allowedCaptureSymbols: true);
      var stringType = compilation.GetSpecialType(SpecialType.System_String);

      // Act
      var result = memberInfo!.IsOfType(stringType);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_true_for_property_with_matching_type()
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
      var memberInfo = InstanceMemberInfo.CreateOrNull(factory, property, populateValueObjectMemberSettings: false, allowedCaptureSymbols: true);
      var stringType = compilation.GetSpecialType(SpecialType.System_String);

      // Act
      var result = memberInfo!.IsOfType(stringType);

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_false_for_property_with_non_matching_type()
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
      var memberInfo = InstanceMemberInfo.CreateOrNull(factory, property, populateValueObjectMemberSettings: false, allowedCaptureSymbols: true);
      var intType = compilation.GetSpecialType(SpecialType.System_Int32);

      // Act
      var result = memberInfo!.IsOfType(intType);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_consider_nullable_annotations_for_reference_types()
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
      var nullableMemberInfo = InstanceMemberInfo.CreateOrNull(factory, nullableField, populateValueObjectMemberSettings: false, allowedCaptureSymbols: true);
      var nonNullableMemberInfo = InstanceMemberInfo.CreateOrNull(factory, nonNullableField, populateValueObjectMemberSettings: false, allowedCaptureSymbols: true);

      // Act
      var nullableResult = nullableMemberInfo!.IsOfType(nullableField.Type);
      var nonNullableResult = nonNullableMemberInfo!.IsOfType(nonNullableField.Type);
      var crossCheckNullable = nullableMemberInfo.IsOfType(nonNullableField.Type);
      var crossCheckNonNullable = nonNullableMemberInfo.IsOfType(nullableField.Type);

      // Assert
      nullableResult.Should().BeTrue();
      nonNullableResult.Should().BeTrue();
      // SymbolEqualityComparer.IncludeNullability considers nullability
      crossCheckNullable.Should().BeFalse();
      crossCheckNonNullable.Should().BeFalse();
   }

   [Fact]
   public void Should_return_true_for_custom_type()
   {
      // Arrange
      var src = @"
namespace Test;

public class CustomType
{
}

public class TestClass
{
   private readonly CustomType _field;
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var testClass = GetTypeSymbol(compilation, "Test.TestClass");
      var customType = GetTypeSymbol(compilation, "Test.CustomType");
      var field = (IFieldSymbol)testClass.GetMembers("_field").First();
      var memberInfo = InstanceMemberInfo.CreateOrNull(factory, field, populateValueObjectMemberSettings: false, allowedCaptureSymbols: true);

      // Act
      var result = memberInfo!.IsOfType(customType);

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_false_for_different_custom_types()
   {
      // Arrange
      var src = @"
namespace Test;

public class CustomType1
{
}

public class CustomType2
{
}

public class TestClass
{
   private readonly CustomType1 _field;
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var testClass = GetTypeSymbol(compilation, "Test.TestClass");
      var customType2 = GetTypeSymbol(compilation, "Test.CustomType2");
      var field = (IFieldSymbol)testClass.GetMembers("_field").First();
      var memberInfo = InstanceMemberInfo.CreateOrNull(factory, field, populateValueObjectMemberSettings: false, allowedCaptureSymbols: true);

      // Act
      var result = memberInfo!.IsOfType(customType2);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_handle_nullable_value_types()
   {
      // Arrange
      var src = @"
namespace Test;

public class TestClass
{
   private readonly int? _nullableField;
   private readonly int _nonNullableField;
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var nullableField = (IFieldSymbol)type.GetMembers("_nullableField").First();
      var nonNullableField = (IFieldSymbol)type.GetMembers("_nonNullableField").First();
      var nullableMemberInfo = InstanceMemberInfo.CreateOrNull(factory, nullableField, populateValueObjectMemberSettings: false, allowedCaptureSymbols: true);
      var nonNullableMemberInfo = InstanceMemberInfo.CreateOrNull(factory, nonNullableField, populateValueObjectMemberSettings: false, allowedCaptureSymbols: true);

      // Act
      var nullableResult = nullableMemberInfo!.IsOfType(nullableField.Type);
      var nonNullableResult = nonNullableMemberInfo!.IsOfType(nonNullableField.Type);
      var crossCheck = nullableMemberInfo.IsOfType(nonNullableField.Type);

      // Assert
      nullableResult.Should().BeTrue();
      nonNullableResult.Should().BeTrue();
      crossCheck.Should().BeFalse();
   }

   [Fact]
   public void Should_return_true_for_array_type()
   {
      // Arrange
      var src = @"
namespace Test;

public class TestClass
{
   private readonly int[] _field;
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var field = (IFieldSymbol)type.GetMembers("_field").First();
      var memberInfo = InstanceMemberInfo.CreateOrNull(factory, field, populateValueObjectMemberSettings: false, allowedCaptureSymbols: true);

      // Act
      var result = memberInfo!.IsOfType(field.Type);

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_false_for_different_array_element_types()
   {
      // Arrange
      var src = @"
namespace Test;

public class TestClass
{
   private readonly int[] _intArrayField;
   private readonly string[] _stringArrayField;
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var intArrayField = (IFieldSymbol)type.GetMembers("_intArrayField").First();
      var stringArrayField = (IFieldSymbol)type.GetMembers("_stringArrayField").First();
      var intArrayMemberInfo = InstanceMemberInfo.CreateOrNull(factory, intArrayField, populateValueObjectMemberSettings: false, allowedCaptureSymbols: true);

      // Act
      var result = intArrayMemberInfo!.IsOfType(stringArrayField.Type);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_true_for_generic_type()
   {
      // Arrange
      var src = @"
using System.Collections.Generic;

namespace Test;

public class TestClass
{
   private readonly List<int> _field;
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var field = (IFieldSymbol)type.GetMembers("_field").First();
      var memberInfo = InstanceMemberInfo.CreateOrNull(factory, field, populateValueObjectMemberSettings: false, allowedCaptureSymbols: true);

      // Act
      var result = memberInfo!.IsOfType(field.Type);

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_false_for_different_generic_type_arguments()
   {
      // Arrange
      var src = @"
using System.Collections.Generic;

namespace Test;

public class TestClass
{
   private readonly List<int> _intListField;
   private readonly List<string> _stringListField;
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var intListField = (IFieldSymbol)type.GetMembers("_intListField").First();
      var stringListField = (IFieldSymbol)type.GetMembers("_stringListField").First();
      var intListMemberInfo = InstanceMemberInfo.CreateOrNull(factory, intListField, populateValueObjectMemberSettings: false, allowedCaptureSymbols: true);

      // Act
      var result = intListMemberInfo!.IsOfType(stringListField.Type);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_work_when_symbol_not_captured()
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
      var intType = compilation.GetSpecialType(SpecialType.System_Int32);

      // Act
      var result = memberInfo!.IsOfType(intType);

      // Assert
      // This should return false because the symbol is not captured (both field and property are null)
      result.Should().BeFalse();
   }
}

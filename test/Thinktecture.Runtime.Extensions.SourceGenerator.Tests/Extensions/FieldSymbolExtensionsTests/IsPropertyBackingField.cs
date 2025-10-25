using System.Linq;
using Microsoft.CodeAnalysis;

#nullable enable

namespace Thinktecture.Runtime.Tests.FieldSymbolExtensionsTests;

public class IsPropertyBackingField : CompilationTestBase
{
   [Fact]
   public void Returns_true_for_auto_property_backing_field()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public string Name { get; set; }
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");

      // Auto-property backing fields are compiler-generated and have names like <Name>k__BackingField
      var backingField = classType.GetMembers()
                                  .OfType<IFieldSymbol>()
                                  .FirstOrDefault(f => f is { IsImplicitlyDeclared: true, AssociatedSymbol: IPropertySymbol });

      backingField.Should().NotBeNull();
      var result = backingField.IsPropertyBackingField();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_for_regular_field()
   {
      var src = @"
namespace Test;

public class MyClass
{
   private string _name;
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var field = classType.GetMembers("_name").OfType<IFieldSymbol>().First();

      var result = field.IsPropertyBackingField();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_field_used_in_property_but_not_backing_field()
   {
      var src = @"
namespace Test;

public class MyClass
{
   private string _name;

   public string Name
   {
      get { return _name; }
      set { _name = value; }
   }
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var field = classType.GetMembers("_name").OfType<IFieldSymbol>().First();

      var result = field.IsPropertyBackingField();

      // Explicit backing fields are not associated with the property via AssociatedSymbol
      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_const_field()
   {
      var src = @"
namespace Test;

public class MyClass
{
   private const string ConstantValue = ""test"";
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var field = classType.GetMembers("ConstantValue").OfType<IFieldSymbol>().First();

      var result = field.IsPropertyBackingField();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_static_field()
   {
      var src = @"
namespace Test;

public class MyClass
{
   private static string _staticField;
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var field = classType.GetMembers("_staticField").OfType<IFieldSymbol>().First();

      var result = field.IsPropertyBackingField();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_readonly_field()
   {
      var src = @"
namespace Test;

public class MyClass
{
   private readonly string _readonlyField;

   public MyClass(string value)
   {
      _readonlyField = value;
   }
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var field = classType.GetMembers("_readonlyField").OfType<IFieldSymbol>().First();

      var result = field.IsPropertyBackingField();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_true_for_auto_property_backing_field_in_class()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public string ClassProperty { get; set; }
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");

      var backingField = classType.GetMembers()
                                  .OfType<IFieldSymbol>()
                                  .FirstOrDefault(f => f is { IsImplicitlyDeclared: true, AssociatedSymbol: IPropertySymbol });

      backingField.Should().NotBeNull();
      var result = backingField.IsPropertyBackingField();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_for_auto_property_backing_field_in_struct()
   {
      var src = @"
namespace Test;

public struct MyStruct
{
   public int StructProperty { get; set; }
}
";
      var compilation = CreateCompilation(src);
      var structType = GetTypeSymbol(compilation, "Test.MyStruct");

      var backingField = structType.GetMembers()
                                   .OfType<IFieldSymbol>()
                                   .FirstOrDefault(f => f is { IsImplicitlyDeclared: true, AssociatedSymbol: IPropertySymbol });

      backingField.Should().NotBeNull();
      var result = backingField.IsPropertyBackingField();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_for_auto_property_backing_field_in_record()
   {
      var src = @"
namespace Test;

public record MyRecord
{
   public string RecordProperty { get; set; }
}
";
      var compilation = CreateCompilation(src);
      var recordType = GetTypeSymbol(compilation, "Test.MyRecord");

      var backingField = recordType.GetMembers()
                                   .OfType<IFieldSymbol>()
                                   .FirstOrDefault(f => f is { IsImplicitlyDeclared: true, AssociatedSymbol: IPropertySymbol { Name: "RecordProperty" } });

      backingField.Should().NotBeNull();
      var result = backingField.IsPropertyBackingField();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_for_property_with_init_accessor_backing_field()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public string Name { get; init; }
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");

      var backingField = classType.GetMembers()
                                  .OfType<IFieldSymbol>()
                                  .FirstOrDefault(f => f is { IsImplicitlyDeclared: true, AssociatedSymbol: IPropertySymbol });

      backingField.Should().NotBeNull();
      var result = backingField.IsPropertyBackingField();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_for_property_with_private_set_backing_field()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public string Name { get; private set; }

   public MyClass(string name)
   {
      Name = name;
   }
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");

      var backingField = classType.GetMembers()
                                  .OfType<IFieldSymbol>()
                                  .FirstOrDefault(f => f is { IsImplicitlyDeclared: true, AssociatedSymbol: IPropertySymbol });

      backingField.Should().NotBeNull();
      var result = backingField.IsPropertyBackingField();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_for_property_with_get_only_backing_field()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public string ReadOnlyProperty { get; }

   public MyClass(string value)
   {
      ReadOnlyProperty = value;
   }
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");

      var backingField = classType.GetMembers()
                                  .OfType<IFieldSymbol>()
                                  .FirstOrDefault(f => f is { IsImplicitlyDeclared: true, AssociatedSymbol: IPropertySymbol });

      backingField.Should().NotBeNull();
      var result = backingField.IsPropertyBackingField();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_for_expression_bodied_property_field()
   {
      var src = @"
namespace Test;

public class MyClass
{
   private int _value = 42;
   public int Value => _value;
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var field = classType.GetMembers("_value").OfType<IFieldSymbol>().First();

      var result = field.IsPropertyBackingField();

      // Expression-bodied properties don't have auto-generated backing fields
      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_public_field()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public string PublicField;
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var field = classType.GetMembers("PublicField").OfType<IFieldSymbol>().First();

      var result = field.IsPropertyBackingField();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_protected_field()
   {
      var src = @"
namespace Test;

public class MyClass
{
   protected string ProtectedField;
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var field = classType.GetMembers("ProtectedField").OfType<IFieldSymbol>().First();

      var result = field.IsPropertyBackingField();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_internal_field()
   {
      var src = @"
namespace Test;

public class MyClass
{
   internal string InternalField;
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var field = classType.GetMembers("InternalField").OfType<IFieldSymbol>().First();

      var result = field.IsPropertyBackingField();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_true_for_static_auto_property_backing_field()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public static string StaticProperty { get; set; }
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");

      var backingField = classType.GetMembers()
                                  .OfType<IFieldSymbol>()
                                  .FirstOrDefault(f => f is { IsImplicitlyDeclared: true, AssociatedSymbol: IPropertySymbol });

      backingField.Should().NotBeNull();
      var result = backingField.IsPropertyBackingField();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_for_virtual_property_backing_field()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public virtual string VirtualProperty { get; set; }
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");

      var backingField = classType.GetMembers()
                                  .OfType<IFieldSymbol>()
                                  .FirstOrDefault(f => f is { IsImplicitlyDeclared: true, AssociatedSymbol: IPropertySymbol });

      backingField.Should().NotBeNull();
      var result = backingField.IsPropertyBackingField();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_for_override_property_backing_field()
   {
      var src = @"
namespace Test;

public abstract class BaseClass
{
   public abstract string Property { get; set; }
}

public class DerivedClass : BaseClass
{
   public override string Property { get; set; }
}
";
      var compilation = CreateCompilation(src);
      var derivedType = GetTypeSymbol(compilation, "Test.DerivedClass");

      var backingField = derivedType.GetMembers()
                                    .OfType<IFieldSymbol>()
                                    .FirstOrDefault(f => f is { IsImplicitlyDeclared: true, AssociatedSymbol: IPropertySymbol });

      backingField.Should().NotBeNull();
      var result = backingField.IsPropertyBackingField();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_for_required_property_backing_field()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public required string RequiredProperty { get; set; }
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");

      var backingField = classType.GetMembers()
                                  .OfType<IFieldSymbol>()
                                  .FirstOrDefault(f => f is { IsImplicitlyDeclared: true, AssociatedSymbol: IPropertySymbol });

      backingField.Should().NotBeNull();
      var result = backingField.IsPropertyBackingField();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_for_property_with_default_value_backing_field()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public string Property { get; set; } = ""default"";
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");

      var backingField = classType.GetMembers()
                                  .OfType<IFieldSymbol>()
                                  .FirstOrDefault(f => f is { IsImplicitlyDeclared: true, AssociatedSymbol: IPropertySymbol });

      backingField.Should().NotBeNull();
      var result = backingField.IsPropertyBackingField();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_for_nullable_reference_type_property_backing_field()
   {
      var src = @"
#nullable enable
namespace Test;

public class MyClass
{
   public string? NullableProperty { get; set; }
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");

      var backingField = classType.GetMembers()
                                  .OfType<IFieldSymbol>()
                                  .FirstOrDefault(f => f is { IsImplicitlyDeclared: true, AssociatedSymbol: IPropertySymbol });

      backingField.Should().NotBeNull();
      var result = backingField.IsPropertyBackingField();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_for_nullable_value_type_property_backing_field()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public int? NullableInt { get; set; }
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");

      var backingField = classType.GetMembers()
                                  .OfType<IFieldSymbol>()
                                  .FirstOrDefault(f => f is { IsImplicitlyDeclared: true, AssociatedSymbol: IPropertySymbol });

      backingField.Should().NotBeNull();
      var result = backingField.IsPropertyBackingField();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_for_generic_type_property_backing_field()
   {
      var src = @"
using System.Collections.Generic;
namespace Test;

public class MyClass
{
   public List<string> GenericProperty { get; set; }
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");

      var backingField = classType.GetMembers()
                                  .OfType<IFieldSymbol>()
                                  .FirstOrDefault(f => f is { IsImplicitlyDeclared: true, AssociatedSymbol: IPropertySymbol });

      backingField.Should().NotBeNull();
      var result = backingField.IsPropertyBackingField();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_for_property_in_generic_class_backing_field()
   {
      var src = @"
namespace Test;

public class MyClass<T>
{
   public T GenericProperty { get; set; }
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass`1");

      var backingField = classType.GetMembers()
                                  .OfType<IFieldSymbol>()
                                  .FirstOrDefault(f => f is { IsImplicitlyDeclared: true, AssociatedSymbol: IPropertySymbol });

      backingField.Should().NotBeNull();
      var result = backingField.IsPropertyBackingField();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_for_nested_class_property_backing_field()
   {
      var src = @"
namespace Test;

public class OuterClass
{
   public class NestedClass
   {
      public string NestedProperty { get; set; }
   }
}
";
      var compilation = CreateCompilation(src);
      var nestedType = GetTypeSymbol(compilation, "Test.OuterClass+NestedClass");

      var backingField = nestedType.GetMembers()
                                   .OfType<IFieldSymbol>()
                                   .FirstOrDefault(f => f is { IsImplicitlyDeclared: true, AssociatedSymbol: IPropertySymbol });

      backingField.Should().NotBeNull();
      var result = backingField.IsPropertyBackingField();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_for_property_with_protected_accessor_backing_field()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public string Property { get; protected set; }
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");

      var backingField = classType.GetMembers()
                                  .OfType<IFieldSymbol>()
                                  .FirstOrDefault(f => f is { IsImplicitlyDeclared: true, AssociatedSymbol: IPropertySymbol });

      backingField.Should().NotBeNull();
      var result = backingField.IsPropertyBackingField();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_for_property_with_internal_accessor_backing_field()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public string Property { get; internal set; }
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");

      var backingField = classType.GetMembers()
                                  .OfType<IFieldSymbol>()
                                  .FirstOrDefault(f => f is { IsImplicitlyDeclared: true, AssociatedSymbol: IPropertySymbol });

      backingField.Should().NotBeNull();
      var result = backingField.IsPropertyBackingField();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_for_record_struct_property_backing_field()
   {
      var src = @"
namespace Test;

public record struct MyRecordStruct
{
   public string Property { get; set; }
}
";
      var compilation = CreateCompilation(src);
      var recordStructType = GetTypeSymbol(compilation, "Test.MyRecordStruct");

      var backingField = recordStructType.GetMembers()
                                         .OfType<IFieldSymbol>()
                                         .FirstOrDefault(f => f is { IsImplicitlyDeclared: true, AssociatedSymbol: IPropertySymbol });

      backingField.Should().NotBeNull();
      var result = backingField.IsPropertyBackingField();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_for_volatile_field()
   {
      var src = @"
namespace Test;

public class MyClass
{
   private volatile int _volatileField;
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var field = classType.GetMembers("_volatileField").OfType<IFieldSymbol>().First();

      var result = field.IsPropertyBackingField();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_field_in_class_without_properties()
   {
      var src = @"
namespace Test;

public class MyClass
{
   private string _field1;
   private int _field2;
   private bool _field3;
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");

      var field1 = classType.GetMembers("_field1").OfType<IFieldSymbol>().First();
      var field2 = classType.GetMembers("_field2").OfType<IFieldSymbol>().First();
      var field3 = classType.GetMembers("_field3").OfType<IFieldSymbol>().First();

      field1.IsPropertyBackingField().Should().BeFalse();
      field2.IsPropertyBackingField().Should().BeFalse();
      field3.IsPropertyBackingField().Should().BeFalse();
   }

   [Fact]
   public void Returns_correct_values_for_multiple_fields_and_properties()
   {
      var src = @"
namespace Test;

public class MyClass
{
   private string _regularField;
   public string AutoProperty { get; set; }
   private int _explicitBackingField;

   public int PropertyWithExplicitBacking
   {
      get { return _explicitBackingField; }
      set { _explicitBackingField = value; }
   }
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");

      var regularField = classType.GetMembers("_regularField").OfType<IFieldSymbol>().First();
      var explicitBackingField = classType.GetMembers("_explicitBackingField").OfType<IFieldSymbol>().First();
      var autoPropertyBackingField = classType.GetMembers()
                                              .OfType<IFieldSymbol>()
                                              .FirstOrDefault(f => f is { IsImplicitlyDeclared: true, AssociatedSymbol: IPropertySymbol });

      regularField.IsPropertyBackingField().Should().BeFalse();
      explicitBackingField.IsPropertyBackingField().Should().BeFalse();
      autoPropertyBackingField!.IsPropertyBackingField().Should().BeTrue();
   }

   [Fact]
   public void Returns_true_for_property_with_attributes_backing_field()
   {
      var src = @"
using System;
namespace Test;

public class MyClass
{
   [Obsolete]
   public string AttributedProperty { get; set; }
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");

      var backingField = classType.GetMembers()
                                  .OfType<IFieldSymbol>()
                                  .FirstOrDefault(f => f is { IsImplicitlyDeclared: true, AssociatedSymbol: IPropertySymbol });

      backingField.Should().NotBeNull();
      var result = backingField.IsPropertyBackingField();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_correct_value_for_associated_property()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public string Name { get; set; }
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");

      var backingField = classType.GetMembers()
                                  .OfType<IFieldSymbol>()
                                  .FirstOrDefault(f => f is { IsImplicitlyDeclared: true, AssociatedSymbol: IPropertySymbol });

      var property = classType.GetMembers("Name").OfType<IPropertySymbol>().First();

      backingField.Should().NotBeNull();
      backingField.AssociatedSymbol.Should().Be(property);
      backingField.IsPropertyBackingField().Should().BeTrue();
   }

   [Fact]
   public void Returns_true_for_field_keyword_property_backing_field()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public string Name
   {
      get => field;
      set => field = value?.Trim();
   }
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");

      var backingField = classType.GetMembers()
                                  .OfType<IFieldSymbol>()
                                  .FirstOrDefault(f => f is { IsImplicitlyDeclared: true, AssociatedSymbol: IPropertySymbol });

      backingField.Should().NotBeNull();
      var result = backingField.IsPropertyBackingField();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_for_field_keyword_property_with_init_accessor()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public string Name
   {
      get => field;
      init => field = value?.ToUpper();
   }
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");

      var backingField = classType.GetMembers()
                                  .OfType<IFieldSymbol>()
                                  .FirstOrDefault(f => f is { IsImplicitlyDeclared: true, AssociatedSymbol: IPropertySymbol });

      backingField.Should().NotBeNull();
      var result = backingField.IsPropertyBackingField();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_for_field_keyword_property_with_get_only()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public string ReadOnlyName
   {
      get => field ?? ""default"";
   }

   public MyClass(string name)
   {
      ReadOnlyName = name;
   }
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");

      var backingField = classType.GetMembers()
                                  .OfType<IFieldSymbol>()
                                  .FirstOrDefault(f => f is { IsImplicitlyDeclared: true, AssociatedSymbol: IPropertySymbol });

      backingField.Should().NotBeNull();
      var result = backingField.IsPropertyBackingField();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_for_field_keyword_property_with_validation()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public int Age
   {
      get => field;
      set => field = value < 0 ? 0 : value;
   }
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");

      var backingField = classType.GetMembers()
                                  .OfType<IFieldSymbol>()
                                  .FirstOrDefault(f => f is { IsImplicitlyDeclared: true, AssociatedSymbol: IPropertySymbol });

      backingField.Should().NotBeNull();
      var result = backingField.IsPropertyBackingField();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_for_field_keyword_property_in_struct()
   {
      var src = @"
namespace Test;

public struct MyStruct
{
   public int Value
   {
      get => field;
      set => field = value;
   }
}
";
      var compilation = CreateCompilation(src);
      var structType = GetTypeSymbol(compilation, "Test.MyStruct");

      var backingField = structType.GetMembers()
                                   .OfType<IFieldSymbol>()
                                   .FirstOrDefault(f => f is { IsImplicitlyDeclared: true, AssociatedSymbol: IPropertySymbol });

      backingField.Should().NotBeNull();
      var result = backingField.IsPropertyBackingField();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_for_field_keyword_property_in_record()
   {
      var src = @"
namespace Test;

public record MyRecord
{
   public string Name
   {
      get => field;
      init => field = value?.Trim();
   }
}
";
      var compilation = CreateCompilation(src);
      var recordType = GetTypeSymbol(compilation, "Test.MyRecord");

      var backingField = recordType.GetMembers()
                                   .OfType<IFieldSymbol>()
                                   .FirstOrDefault(f => f is { IsImplicitlyDeclared: true, AssociatedSymbol: IPropertySymbol });

      backingField.Should().NotBeNull();
      var result = backingField.IsPropertyBackingField();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_for_field_keyword_property_with_nullable_reference_type()
   {
      var src = @"
#nullable enable
namespace Test;

public class MyClass
{
   public string? NullableName
   {
      get => field;
      set => field = value?.Trim();
   }
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");

      var backingField = classType.GetMembers()
                                  .OfType<IFieldSymbol>()
                                  .FirstOrDefault(f => f is { IsImplicitlyDeclared: true, AssociatedSymbol: IPropertySymbol });

      backingField.Should().NotBeNull();
      var result = backingField.IsPropertyBackingField();

      result.Should().BeTrue();
   }

}

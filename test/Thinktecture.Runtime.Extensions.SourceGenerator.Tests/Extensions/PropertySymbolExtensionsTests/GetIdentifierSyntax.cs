using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;

#nullable enable

namespace Thinktecture.Runtime.Tests.PropertySymbolExtensionsTests;

public class GetIdentifierSyntax : CompilationTestBase
{
   [Fact]
   public void Returns_identifier_for_regular_property_with_getter_and_setter()
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
      var property = classType.GetMembers("Name").OfType<IPropertySymbol>().First();

      var result = property.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("Name");
   }

   [Fact]
   public void Returns_identifier_for_auto_property()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public int Age { get; set; }
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var property = classType.GetMembers("Age").OfType<IPropertySymbol>().First();

      var result = property.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("Age");
   }

   [Fact]
   public void Returns_identifier_for_expression_bodied_property()
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
      var property = classType.GetMembers("Value").OfType<IPropertySymbol>().First();

      var result = property.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("Value");
   }

   [Fact]
   public void Returns_identifier_for_property_with_only_getter()
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
      var property = classType.GetMembers("ReadOnlyProperty").OfType<IPropertySymbol>().First();

      var result = property.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("ReadOnlyProperty");
   }

   [Fact]
   public void Returns_identifier_for_property_with_private_setter()
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
      var property = classType.GetMembers("Name").OfType<IPropertySymbol>().First();

      var result = property.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("Name");
   }

   [Fact]
   public void Returns_identifier_for_property_with_init_accessor()
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
      var property = classType.GetMembers("Name").OfType<IPropertySymbol>().First();

      var result = property.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("Name");
   }

   [Fact]
   public void Returns_identifier_for_property_in_class()
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
      var property = classType.GetMembers("ClassProperty").OfType<IPropertySymbol>().First();

      var result = property.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("ClassProperty");
   }

   [Fact]
   public void Returns_identifier_for_property_in_struct()
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
      var property = structType.GetMembers("StructProperty").OfType<IPropertySymbol>().First();

      var result = property.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("StructProperty");
   }

   [Fact]
   public void Returns_identifier_for_property_in_interface()
   {
      var src = @"
namespace Test;

public interface IMyInterface
{
   string InterfaceProperty { get; set; }
}
";
      var compilation = CreateCompilation(src);
      var interfaceType = GetTypeSymbol(compilation, "Test.IMyInterface");
      var property = interfaceType.GetMembers("InterfaceProperty").OfType<IPropertySymbol>().First();

      var result = property.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("InterfaceProperty");
   }

   [Fact]
   public void Returns_identifier_for_property_in_record()
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
      var property = recordType.GetMembers("RecordProperty").OfType<IPropertySymbol>().First();

      var result = property.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("RecordProperty");
   }

   [Fact]
   public void Returns_identifier_for_property_with_backing_field()
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
      var property = classType.GetMembers("Name").OfType<IPropertySymbol>().First();

      var result = property.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("Name");
   }

   [Fact]
   public void Returns_identifier_for_static_property()
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
      var property = classType.GetMembers("StaticProperty").OfType<IPropertySymbol>().First();

      var result = property.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("StaticProperty");
   }

   [Fact]
   public void Returns_identifier_for_virtual_property()
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
      var property = classType.GetMembers("VirtualProperty").OfType<IPropertySymbol>().First();

      var result = property.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("VirtualProperty");
   }

   [Fact]
   public void Returns_identifier_for_abstract_property()
   {
      var src = @"
namespace Test;

public abstract class MyClass
{
   public abstract string AbstractProperty { get; set; }
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var property = classType.GetMembers("AbstractProperty").OfType<IPropertySymbol>().First();

      var result = property.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("AbstractProperty");
   }

   [Fact]
   public void Returns_identifier_for_override_property()
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
      var property = derivedType.GetMembers("Property").OfType<IPropertySymbol>().First();

      var result = property.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("Property");
   }

   [Fact]
   public void Returns_identifier_for_property_with_complex_getter()
   {
      var src = @"
namespace Test;

public class MyClass
{
   private int _value = 10;

   public int ComplexProperty
   {
      get
      {
         if (_value > 0)
            return _value * 2;
         return 0;
      }
   }
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var property = classType.GetMembers("ComplexProperty").OfType<IPropertySymbol>().First();

      var result = property.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("ComplexProperty");
   }

   [Fact]
   public void Returns_identifier_for_property_with_attributes()
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
      var property = classType.GetMembers("AttributedProperty").OfType<IPropertySymbol>().First();

      var result = property.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("AttributedProperty");
   }

   [Fact]
   public void Returns_identifier_for_property_with_nullable_reference_type()
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
      var property = classType.GetMembers("NullableProperty").OfType<IPropertySymbol>().First();

      var result = property.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("NullableProperty");
   }

   [Fact]
   public void Returns_identifier_for_property_with_nullable_value_type()
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
      var property = classType.GetMembers("NullableInt").OfType<IPropertySymbol>().First();

      var result = property.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("NullableInt");
   }

   [Fact]
   public void Returns_identifier_for_property_with_generic_type()
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
      var property = classType.GetMembers("GenericProperty").OfType<IPropertySymbol>().First();

      var result = property.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("GenericProperty");
   }

   [Fact]
   public void Returns_identifier_for_property_in_generic_class()
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
      var property = classType.GetMembers("GenericProperty").OfType<IPropertySymbol>().First();

      var result = property.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("GenericProperty");
   }

   [Fact]
   public void Returns_identifier_for_nested_class_property()
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
      var property = nestedType.GetMembers("NestedProperty").OfType<IPropertySymbol>().First();

      var result = property.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("NestedProperty");
   }

   [Fact]
   public void Returns_identifier_for_property_with_private_accessor()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public string Property { private get; set; }
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var property = classType.GetMembers("Property").OfType<IPropertySymbol>().First();

      var result = property.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("Property");
   }

   [Fact]
   public void Returns_identifier_for_property_with_protected_accessor()
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
      var property = classType.GetMembers("Property").OfType<IPropertySymbol>().First();

      var result = property.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("Property");
   }

   [Fact]
   public void Returns_identifier_for_property_with_internal_accessor()
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
      var property = classType.GetMembers("Property").OfType<IPropertySymbol>().First();

      var result = property.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("Property");
   }

   [Fact]
   public void Returns_identifier_for_required_property()
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
      var property = classType.GetMembers("RequiredProperty").OfType<IPropertySymbol>().First();

      var result = property.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("RequiredProperty");
   }

   [Fact]
   public void Returns_identifier_for_property_with_default_value()
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
      var property = classType.GetMembers("Property").OfType<IPropertySymbol>().First();

      var result = property.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("Property");
   }

   [Fact]
   public void Returns_null_for_indexer_property()
   {
      var src = @"
namespace Test;

public class MyClass
{
   private string[] _values = new string[10];

   public string this[int index]
   {
      get { return _values[index]; }
      set { _values[index] = value; }
   }
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var indexer = classType.GetMembers().OfType<IPropertySymbol>().First(p => p.IsIndexer);

      var result = indexer.GetIdentifierSyntax(CancellationToken.None);

      // Indexers use IndexerDeclarationSyntax, not PropertyDeclarationSyntax, so GetIdentifier returns null
      result.Should().BeNull();
   }

   [Fact]
   public void Returns_identifier_with_correct_text_for_property_with_underscores()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public string _Under_Score_Property { get; set; }
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var property = classType.GetMembers("_Under_Score_Property").OfType<IPropertySymbol>().First();

      var result = property.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("_Under_Score_Property");
   }

   [Fact]
   public void Returns_identifier_for_property_with_escaped_identifier()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public string @class { get; set; }
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var property = classType.GetMembers("class").OfType<IPropertySymbol>().First();

      var result = property.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      // SyntaxToken.Text includes the @ symbol for escaped identifiers
      result.Value.Text.Should().Be("@class");
   }

   [Fact]
   public void Returns_identifier_for_property_in_record_struct()
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
      var property = recordStructType.GetMembers("Property").OfType<IPropertySymbol>().First();

      var result = property.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("Property");
   }

   [Fact]
   public void Returns_identifier_for_property_with_multiple_syntax_references()
   {
      var src = @"
namespace Test;

public partial class MyClass
{
   public string Property { get; set; }
}

public partial class MyClass;
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var property = classType.GetMembers("Property").OfType<IPropertySymbol>().First();

      var result = property.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("Property");
   }

   [Fact]
   public void Returns_null_for_implicitly_defined_property_without_syntax()
   {
      var src = @"
namespace Test;

public interface IBase
{
   string Property { get; set; }
}

public class Derived : IBase
{
   string IBase.Property { get; set; }
}
";
      var compilation = CreateCompilation(src);
      var baseInterface = GetTypeSymbol(compilation, "Test.IBase");
      var baseProperty = baseInterface.GetMembers("Property").OfType<IPropertySymbol>().First();

      // Get the derived class and find the interface property implementation
      var derivedClass = GetTypeSymbol(compilation, "Test.Derived");
      var implementingProperty = derivedClass.GetMembers().OfType<IPropertySymbol>()
         .FirstOrDefault(p => SymbolEqualityComparer.Default.Equals(p.ExplicitInterfaceImplementations.FirstOrDefault(), baseProperty));

      var result = implementingProperty!.GetIdentifierSyntax(CancellationToken.None);
      result.Should().NotBeNull();
      result.Value.Text.Should().Be("Property");
   }

   [Fact]
   public void Returns_identifier_for_record_positional_parameter()
   {
      var src = @"
namespace Test;

public record Person(string Name, int Age);
";
      var compilation = CreateCompilation(src);
      var recordType = GetTypeSymbol(compilation, "Test.Person");
      var property = recordType.GetMembers("Name").OfType<IPropertySymbol>().First();

      var result = property.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("Name");
   }

   [Fact]
   public void Returns_identifier_for_record_struct_positional_parameter()
   {
      var src = @"
namespace Test;

public record struct Point(int X, int Y);
";
      var compilation = CreateCompilation(src);
      var recordStructType = GetTypeSymbol(compilation, "Test.Point");
      var property = recordStructType.GetMembers("X").OfType<IPropertySymbol>().First();

      var result = property.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("X");
   }

   [Fact]
   public void Returns_identifier_for_record_with_multiple_positional_parameters()
   {
      var src = @"
namespace Test;

public record Product(string Name, decimal Price, int Quantity);
";
      var compilation = CreateCompilation(src);
      var recordType = GetTypeSymbol(compilation, "Test.Product");

      var nameProperty = recordType.GetMembers("Name").OfType<IPropertySymbol>().First();
      var priceProperty = recordType.GetMembers("Price").OfType<IPropertySymbol>().First();
      var quantityProperty = recordType.GetMembers("Quantity").OfType<IPropertySymbol>().First();

      var nameResult = nameProperty.GetIdentifierSyntax(CancellationToken.None);
      var priceResult = priceProperty.GetIdentifierSyntax(CancellationToken.None);
      var quantityResult = quantityProperty.GetIdentifierSyntax(CancellationToken.None);

      nameResult.Should().NotBeNull();
      nameResult.Value.Text.Should().Be("Name");

      priceResult.Should().NotBeNull();
      priceResult.Value.Text.Should().Be("Price");

      quantityResult.Should().NotBeNull();
      quantityResult.Value.Text.Should().Be("Quantity");
   }

   [Fact]
   public void Returns_identifier_for_record_with_mixed_properties()
   {
      var src = @"
namespace Test;

public record Person(string FirstName, string LastName)
{
   public int Age { get; set; }
}
";
      var compilation = CreateCompilation(src);
      var recordType = GetTypeSymbol(compilation, "Test.Person");

      var firstNameProperty = recordType.GetMembers("FirstName").OfType<IPropertySymbol>().First();
      var lastNameProperty = recordType.GetMembers("LastName").OfType<IPropertySymbol>().First();
      var ageProperty = recordType.GetMembers("Age").OfType<IPropertySymbol>().First();

      var firstNameResult = firstNameProperty.GetIdentifierSyntax(CancellationToken.None);
      var lastNameResult = lastNameProperty.GetIdentifierSyntax(CancellationToken.None);
      var ageResult = ageProperty.GetIdentifierSyntax(CancellationToken.None);

      firstNameResult.Should().NotBeNull();
      firstNameResult.Value.Text.Should().Be("FirstName");

      lastNameResult.Should().NotBeNull();
      lastNameResult.Value.Text.Should().Be("LastName");

      ageResult.Should().NotBeNull();
      ageResult.Value.Text.Should().Be("Age");
   }

}

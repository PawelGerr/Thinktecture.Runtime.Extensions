using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;

#nullable enable

namespace Thinktecture.Runtime.Tests.FieldSymbolExtensionsTests;

public class GetIdentifierSyntax : CompilationTestBase
{
   [Fact]
   public void Returns_identifier_for_regular_field()
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

      var result = field.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("_name");
   }

   [Fact]
   public void Returns_identifier_for_static_field()
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

      var result = field.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("_staticField");
   }

   [Fact]
   public void Returns_identifier_for_readonly_field()
   {
      var src = @"
namespace Test;

public class MyClass
{
   private readonly string _readonlyField;
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var field = classType.GetMembers("_readonlyField").OfType<IFieldSymbol>().First();

      var result = field.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("_readonlyField");
   }

   [Fact]
   public void Returns_identifier_for_const_field()
   {
      var src = @"
namespace Test;

public class MyClass
{
   private const string ConstField = ""constant"";
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var field = classType.GetMembers("ConstField").OfType<IFieldSymbol>().First();

      var result = field.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("ConstField");
   }

   [Fact]
   public void Returns_identifier_for_private_field()
   {
      var src = @"
namespace Test;

public class MyClass
{
   private int _privateField;
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var field = classType.GetMembers("_privateField").OfType<IFieldSymbol>().First();

      var result = field.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("_privateField");
   }

   [Fact]
   public void Returns_identifier_for_public_field()
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

      var result = field.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("PublicField");
   }

   [Fact]
   public void Returns_identifier_for_protected_field()
   {
      var src = @"
namespace Test;

public class MyClass
{
   protected string _protectedField;
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var field = classType.GetMembers("_protectedField").OfType<IFieldSymbol>().First();

      var result = field.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("_protectedField");
   }

   [Fact]
   public void Returns_identifier_for_internal_field()
   {
      var src = @"
namespace Test;

public class MyClass
{
   internal string _internalField;
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var field = classType.GetMembers("_internalField").OfType<IFieldSymbol>().First();

      var result = field.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("_internalField");
   }

   [Fact]
   public void Returns_identifier_for_field_with_initializer()
   {
      var src = @"
namespace Test;

public class MyClass
{
   private string _fieldWithInitializer = ""initial value"";
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var field = classType.GetMembers("_fieldWithInitializer").OfType<IFieldSymbol>().First();

      var result = field.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("_fieldWithInitializer");
   }

   [Fact]
   public void Returns_identifier_for_field_in_class()
   {
      var src = @"
namespace Test;

public class MyClass
{
   private string _classField;
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var field = classType.GetMembers("_classField").OfType<IFieldSymbol>().First();

      var result = field.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("_classField");
   }

   [Fact]
   public void Returns_identifier_for_field_in_struct()
   {
      var src = @"
namespace Test;

public struct MyStruct
{
   private int _structField;
}
";
      var compilation = CreateCompilation(src);
      var structType = GetTypeSymbol(compilation, "Test.MyStruct");
      var field = structType.GetMembers("_structField").OfType<IFieldSymbol>().First();

      var result = field.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("_structField");
   }

   [Fact]
   public void Returns_identifier_for_field_in_record()
   {
      var src = @"
namespace Test;

public record MyRecord
{
   private string _recordField;
}
";
      var compilation = CreateCompilation(src);
      var recordType = GetTypeSymbol(compilation, "Test.MyRecord");
      var field = recordType.GetMembers("_recordField").OfType<IFieldSymbol>().First();

      var result = field.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("_recordField");
   }

   [Fact]
   public void Returns_identifier_for_field_with_attributes()
   {
      var src = @"
using System;
namespace Test;

public class MyClass
{
   [Obsolete]
   private string _attributedField;
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var field = classType.GetMembers("_attributedField").OfType<IFieldSymbol>().First();

      var result = field.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("_attributedField");
   }

   [Fact]
   public void Returns_identifier_for_field_with_nullable_reference_type()
   {
      var src = @"
#nullable enable
namespace Test;

public class MyClass
{
   private string? _nullableField;
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var field = classType.GetMembers("_nullableField").OfType<IFieldSymbol>().First();

      var result = field.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("_nullableField");
   }

   [Fact]
   public void Returns_identifier_for_field_with_nullable_value_type()
   {
      var src = @"
namespace Test;

public class MyClass
{
   private int? _nullableInt;
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var field = classType.GetMembers("_nullableInt").OfType<IFieldSymbol>().First();

      var result = field.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("_nullableInt");
   }

   [Fact]
   public void Returns_identifier_for_first_field_in_multiple_field_declaration()
   {
      var src = @"
namespace Test;

public class MyClass
{
   private int x, y, z;
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var fieldX = classType.GetMembers("x").OfType<IFieldSymbol>().First();

      var result = fieldX.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("x");
   }

   [Fact]
   public void Returns_identifier_for_second_field_in_multiple_field_declaration()
   {
      var src = @"
namespace Test;

public class MyClass
{
   private int x, y, z;
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var fieldY = classType.GetMembers("y").OfType<IFieldSymbol>().First();

      var result = fieldY.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("y");
   }

   [Fact]
   public void Returns_identifier_for_third_field_in_multiple_field_declaration()
   {
      var src = @"
namespace Test;

public class MyClass
{
   private int x, y, z;
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var fieldZ = classType.GetMembers("z").OfType<IFieldSymbol>().First();

      var result = fieldZ.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("z");
   }

   [Fact]
   public void Returns_identifier_for_field_in_generic_class()
   {
      var src = @"
namespace Test;

public class MyClass<T>
{
   private T _genericField;
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass`1");
      var field = classType.GetMembers("_genericField").OfType<IFieldSymbol>().First();

      var result = field.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("_genericField");
   }

   [Fact]
   public void Returns_identifier_for_field_in_nested_class()
   {
      var src = @"
namespace Test;

public class OuterClass
{
   public class NestedClass
   {
      private string _nestedField;
   }
}
";
      var compilation = CreateCompilation(src);
      var nestedType = GetTypeSymbol(compilation, "Test.OuterClass+NestedClass");
      var field = nestedType.GetMembers("_nestedField").OfType<IFieldSymbol>().First();

      var result = field.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("_nestedField");
   }

   [Fact]
   public void Returns_identifier_for_field_with_escaped_identifier()
   {
      var src = @"
namespace Test;

public class MyClass
{
   private string @class;
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var field = classType.GetMembers("class").OfType<IFieldSymbol>().First();

      var result = field.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      // SyntaxToken.Text includes the @ symbol for escaped identifiers
      result.Value.Text.Should().Be("@class");
   }

   [Fact]
   public void Returns_identifier_for_static_readonly_field()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public static readonly string StaticReadonlyField = ""value"";
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var field = classType.GetMembers("StaticReadonlyField").OfType<IFieldSymbol>().First();

      var result = field.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("StaticReadonlyField");
   }

   [Fact]
   public void Returns_identifier_for_volatile_field()
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

      var result = field.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("_volatileField");
   }

   [Fact]
   public void Returns_identifier_for_field_with_generic_type()
   {
      var src = @"
using System.Collections.Generic;
namespace Test;

public class MyClass
{
   private List<string> _genericTypeField;
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var field = classType.GetMembers("_genericTypeField").OfType<IFieldSymbol>().First();

      var result = field.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("_genericTypeField");
   }

   [Fact]
   public void Returns_identifier_for_field_in_record_struct()
   {
      var src = @"
namespace Test;

public record struct MyRecordStruct
{
   private int _recordStructField;
}
";
      var compilation = CreateCompilation(src);
      var recordStructType = GetTypeSymbol(compilation, "Test.MyRecordStruct");
      var field = recordStructType.GetMembers("_recordStructField").OfType<IFieldSymbol>().First();

      var result = field.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("_recordStructField");
   }

   [Fact]
   public void Returns_identifier_for_field_with_underscores()
   {
      var src = @"
namespace Test;

public class MyClass
{
   private string _under_score_field;
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var field = classType.GetMembers("_under_score_field").OfType<IFieldSymbol>().First();

      var result = field.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("_under_score_field");
   }

   [Fact]
   public void Returns_identifier_for_protected_internal_field()
   {
      var src = @"
namespace Test;

public class MyClass
{
   protected internal string _protectedInternalField;
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var field = classType.GetMembers("_protectedInternalField").OfType<IFieldSymbol>().First();

      var result = field.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("_protectedInternalField");
   }

   [Fact]
   public void Returns_identifier_for_private_protected_field()
   {
      var src = @"
namespace Test;

public class MyClass
{
   private protected string _privateProtectedField;
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var field = classType.GetMembers("_privateProtectedField").OfType<IFieldSymbol>().First();

      var result = field.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("_privateProtectedField");
   }

   [Fact]
   public void Returns_identifier_for_field_with_complex_initializer()
   {
      var src = @"
namespace Test;

public class MyClass
{
   private int _fieldWithComplexInitializer = 10 + 20 * 30;
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var field = classType.GetMembers("_fieldWithComplexInitializer").OfType<IFieldSymbol>().First();

      var result = field.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("_fieldWithComplexInitializer");
   }

   [Fact]
   public void Returns_identifier_for_field_with_new_keyword()
   {
      var src = @"
namespace Test;

public class BaseClass
{
   protected string _field;
}

public class DerivedClass : BaseClass
{
   private new string _field;
}
";
      var compilation = CreateCompilation(src);
      var derivedType = GetTypeSymbol(compilation, "Test.DerivedClass");
      var field = derivedType.GetMembers("_field").OfType<IFieldSymbol>().First();

      var result = field.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("_field");
   }

   [Fact]
   public void Returns_identifier_for_field_with_array_type()
   {
      var src = @"
namespace Test;

public class MyClass
{
   private string[] _arrayField;
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var field = classType.GetMembers("_arrayField").OfType<IFieldSymbol>().First();

      var result = field.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("_arrayField");
   }

   [Fact]
   public void Returns_identifier_for_required_field()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public required string RequiredField;
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var field = classType.GetMembers("RequiredField").OfType<IFieldSymbol>().First();

      var result = field.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("RequiredField");
   }

   [Fact]
   public void Returns_identifier_for_field_in_partial_class()
   {
      var src = @"
namespace Test;

public partial class MyClass
{
   private string _partialField;
}

public partial class MyClass;
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var field = classType.GetMembers("_partialField").OfType<IFieldSymbol>().First();

      var result = field.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("_partialField");
   }

   [Fact]
   public void Returns_identifier_for_field_with_tuple_type()
   {
      var src = @"
namespace Test;

public class MyClass
{
   private (int, string) _tupleField;
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var field = classType.GetMembers("_tupleField").OfType<IFieldSymbol>().First();

      var result = field.GetIdentifierSyntax(CancellationToken.None);

      result.Should().NotBeNull();
      result.Value.Text.Should().Be("_tupleField");
   }

   [Fact]
   public void Returns_null_for_field_keyword_property_backing_field()
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

      // The field keyword creates a compiler-generated backing field
      var backingField = classType.GetMembers()
                                  .OfType<IFieldSymbol>()
                                  .FirstOrDefault(f => f is { IsImplicitlyDeclared: true, AssociatedSymbol: IPropertySymbol });

      backingField.Should().NotBeNull();
      var result = backingField.GetIdentifierSyntax(CancellationToken.None);

      result.Should().BeNull();
   }

   [Fact]
   public void Returns_identifier_for_field_keyword_property_with_init_accessor()
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
      var result = backingField.GetIdentifierSyntax(CancellationToken.None);

      result.Should().BeNull();
   }

   [Fact]
   public void Returns_identifier_for_field_keyword_property_with_get_only()
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
      var result = backingField.GetIdentifierSyntax(CancellationToken.None);

      result.Should().BeNull();
   }
}

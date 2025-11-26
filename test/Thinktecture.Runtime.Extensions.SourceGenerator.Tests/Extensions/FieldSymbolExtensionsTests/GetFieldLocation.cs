using System;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Thinktecture.Runtime.Tests.FieldSymbolExtensionsTests;

public class GetFieldLocation : CompilationTestBase
{
   [Fact]
   public void Should_delegate_to_property_location_for_auto_property_backing_field()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public int Value { get; set; }
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var property = type.GetMembers("Value").OfType<IPropertySymbol>().Single();
      var backingField = type.GetMembers().OfType<IFieldSymbol>().FirstOrDefault(f => SymbolEqualityComparer.Default.Equals(f.AssociatedSymbol, property));

      backingField.Should().NotBeNull("auto-property should have a backing field");
      var location = backingField!.GetFieldLocation(CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
      // The location should point to the property identifier
      var sourceText = location.SourceTree?.GetText(TestContext.Current.CancellationToken).ToString();
      var locationText = sourceText?.Substring(location.SourceSpan.Start, location.SourceSpan.Length);
      locationText.Should().Be("Value");
   }

   [Fact]
   public void Should_not_delegate_to_event_location_for_explicit_event_backing_field()
   {
      var src = @"
using System;

namespace Test;

public class MyClass
{
   private EventHandler _myEvent;

   public event EventHandler MyEvent
   {
      add { _myEvent += value; }
      remove { _myEvent -= value; }
   }
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      // For property-like events with custom accessors, the backing field is explicit
      var backingField = type.GetMembers("_myEvent").OfType<IFieldSymbol>().Single();

      // Verify the backing field is not implicitly declared and test it
      backingField.IsImplicitlyDeclared.Should().BeFalse();
      var location = backingField.GetFieldLocation(CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
      // The location should point to the field identifier
      var sourceText = location.SourceTree?.GetText(TestContext.Current.CancellationToken).ToString();
      var locationText = sourceText?.Substring(location.SourceSpan.Start, location.SourceSpan.Length);
      locationText.Should().Be("_myEvent");
   }

   [Fact]
   public void Should_return_containing_type_location_for_other_synthesized_fields()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public int Value { get; }

   public MyClass(int value)
   {
      Value = value;
   }
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var property = type.GetMembers("Value").OfType<IPropertySymbol>().Single();
      var backingField = type.GetMembers().OfType<IFieldSymbol>().FirstOrDefault(f => SymbolEqualityComparer.Default.Equals(f.AssociatedSymbol, property));

      backingField.Should().NotBeNull("property should have a backing field");
      var location = backingField!.GetFieldLocation(CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_return_identifier_location_for_single_field_declaration()
   {
      var src = @"
namespace Test;

public class MyClass
{
   private int _value;
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var field = type.GetMembers("_value").OfType<IFieldSymbol>().Single();

      var location = field.GetFieldLocation(CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
      var sourceText = location.SourceTree?.GetText(TestContext.Current.CancellationToken).ToString();
      var locationText = sourceText?.Substring(location.SourceSpan.Start, location.SourceSpan.Length);
      locationText.Should().Be("_value");
   }

   [Fact]
   public void Should_return_identifier_location_for_first_field_in_multiple_declarators()
   {
      var src = @"
namespace Test;

public class MyClass
{
   private int _field1, _field2, _field3;
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var field1 = type.GetMembers("_field1").OfType<IFieldSymbol>().Single();

      var location = field1.GetFieldLocation(CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
      var sourceText = location.SourceTree?.GetText(TestContext.Current.CancellationToken).ToString();
      var locationText = sourceText?.Substring(location.SourceSpan.Start, location.SourceSpan.Length);
      locationText.Should().Be("_field1");
   }

   [Fact]
   public void Should_return_identifier_location_for_second_field_in_multiple_declarators()
   {
      var src = @"
namespace Test;

public class MyClass
{
   private int _field1, _field2, _field3;
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var field2 = type.GetMembers("_field2").OfType<IFieldSymbol>().Single();

      var location = field2.GetFieldLocation(CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
      var sourceText = location.SourceTree?.GetText(TestContext.Current.CancellationToken).ToString();
      var locationText = sourceText?.Substring(location.SourceSpan.Start, location.SourceSpan.Length);
      locationText.Should().Be("_field2");
   }

   [Fact]
   public void Should_return_identifier_location_for_third_field_in_multiple_declarators()
   {
      var src = @"
namespace Test;

public class MyClass
{
   private int _field1, _field2, _field3;
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var field3 = type.GetMembers("_field3").OfType<IFieldSymbol>().Single();

      var location = field3.GetFieldLocation(CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
      var sourceText = location.SourceTree?.GetText(TestContext.Current.CancellationToken).ToString();
      var locationText = sourceText?.Substring(location.SourceSpan.Start, location.SourceSpan.Length);
      locationText.Should().Be("_field3");
   }

   [Fact]
   public void Should_handle_field_with_initializer()
   {
      var src = @"
namespace Test;

public class MyClass
{
   private int _value = 42;
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var field = type.GetMembers("_value").OfType<IFieldSymbol>().Single();

      var location = field.GetFieldLocation(CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
      var sourceText = location.SourceTree?.GetText(TestContext.Current.CancellationToken).ToString();
      var locationText = sourceText?.Substring(location.SourceSpan.Start, location.SourceSpan.Length);
      locationText.Should().Be("_value");
   }

   [Theory]
   [InlineData("public")]
   [InlineData("private")]
   [InlineData("protected")]
   [InlineData("internal")]
   [InlineData("protected internal")]
   [InlineData("private protected")]
   public void Should_handle_field_with_different_access_modifiers(string accessModifier)
   {
      var src = $@"
namespace Test;

public class MyClass
{{
   {accessModifier} int _field;
}}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var field = type.GetMembers("_field").OfType<IFieldSymbol>().Single();

      var location = field.GetFieldLocation(CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_static_field()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public static int StaticField;
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var field = type.GetMembers("StaticField").OfType<IFieldSymbol>().Single();

      var location = field.GetFieldLocation(CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_readonly_field()
   {
      var src = @"
namespace Test;

public class MyClass
{
   private readonly int _readonly;

   public MyClass(int value)
   {
      _readonly = value;
   }
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var field = type.GetMembers("_readonly").OfType<IFieldSymbol>().Single();

      var location = field.GetFieldLocation(CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_const_field()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public const int MaxValue = 100;
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var field = type.GetMembers("MaxValue").OfType<IFieldSymbol>().Single();

      var location = field.GetFieldLocation(CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
      var sourceText = location.SourceTree?.GetText(TestContext.Current.CancellationToken).ToString();
      var locationText = sourceText?.Substring(location.SourceSpan.Start, location.SourceSpan.Length);
      locationText.Should().Be("MaxValue");
   }

   [Fact]
   public void Should_handle_volatile_field()
   {
      var src = @"
namespace Test;

public class MyClass
{
   private volatile int _volatileField;
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var field = type.GetMembers("_volatileField").OfType<IFieldSymbol>().Single();

      var location = field.GetFieldLocation(CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_return_identifier_location_for_enum_member()
   {
      var src = @"
namespace Test;

public enum MyEnum
{
   First,
   Second,
   Third
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyEnum");
      var firstMember = type.GetMembers("First").OfType<IFieldSymbol>().Single();

      var location = firstMember.GetFieldLocation(CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
      var sourceText = location.SourceTree?.GetText(TestContext.Current.CancellationToken).ToString();
      var locationText = sourceText?.Substring(location.SourceSpan.Start, location.SourceSpan.Length);
      locationText.Should().Be("First");
   }

   [Fact]
   public void Should_return_identifier_location_for_enum_member_with_explicit_value()
   {
      var src = @"
namespace Test;

public enum MyEnum
{
   First = 1,
   Second = 2,
   Third = 4
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyEnum");
      var secondMember = type.GetMembers("Second").OfType<IFieldSymbol>().Single();

      var location = secondMember.GetFieldLocation(CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
      var sourceText = location.SourceTree?.GetText(TestContext.Current.CancellationToken).ToString();
      var locationText = sourceText?.Substring(location.SourceSpan.Start, location.SourceSpan.Length);
      locationText.Should().Be("Second");
   }

   [Fact]
   public void Should_handle_enum_member_with_attributes()
   {
      var src = @"
using System;
using System.ComponentModel;

namespace Test;

public enum MyEnum
{
   [Description(""First value"")]
   First = 1,

   [Obsolete]
   Second = 2
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyEnum");
      var firstMember = type.GetMembers("First").OfType<IFieldSymbol>().Single();

      var location = firstMember.GetFieldLocation(CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
      var sourceText = location.SourceTree?.GetText(TestContext.Current.CancellationToken).ToString();
      var locationText = sourceText?.Substring(location.SourceSpan.Start, location.SourceSpan.Length);
      locationText.Should().Be("First");
   }

   [Fact]
   public void Should_handle_flags_enum()
   {
      var src = @"
using System;

namespace Test;

[Flags]
public enum MyFlags
{
   None = 0,
   Read = 1,
   Write = 2,
   Execute = 4,
   All = Read | Write | Execute
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyFlags");
      var allMember = type.GetMembers("All").OfType<IFieldSymbol>().Single();

      var location = allMember.GetFieldLocation(CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
      var sourceText = location.SourceTree?.GetText(TestContext.Current.CancellationToken).ToString();
      var locationText = sourceText?.Substring(location.SourceSpan.Start, location.SourceSpan.Length);
      locationText.Should().Be("All");
   }

   [Fact]
   public void Should_skip_generated_tree_and_return_non_generated_location()
   {
      var userSource = @"
namespace Test;

public partial class MyClass
{
   private int _userField;
}
";
      var generatedSource = @"
namespace Test;

public partial class MyClass
{
   private int _generatedField;
}
";
      var userTree = CSharpSyntaxTree.ParseText(userSource, path: "MyClass.cs", cancellationToken: TestContext.Current.CancellationToken);
      var generatedTree = CSharpSyntaxTree.ParseText(generatedSource, path: "MyClass.g.cs", cancellationToken: TestContext.Current.CancellationToken);

      var compilation = CSharpCompilation.Create(
         "TestAssembly",
         [userTree, generatedTree],
         AppDomain.CurrentDomain.GetAssemblies()
                  .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
                  .Select(a => MetadataReference.CreateFromFile(a.Location)),
         new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

      var type = compilation.GetTypeByMetadataName("Test.MyClass");
      type.Should().NotBeNull();
      var userField = type!.GetMembers("_userField").OfType<IFieldSymbol>().Single();

      var location = userField.GetFieldLocation(CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
      location.SourceTree?.FilePath.Should().Be("MyClass.cs");
      location.SourceTree?.FilePath.Should().NotContain(".g.cs");
   }

   [Fact]
   public void Should_return_Location_None_when_all_trees_are_generated()
   {
      var generatedSource = @"
namespace Test;

public class MyClass
{
   private int _generatedField;
}
";
      var generatedTree = CSharpSyntaxTree.ParseText(generatedSource, path: "MyClass.g.cs", cancellationToken: TestContext.Current.CancellationToken);

      var compilation = CSharpCompilation.Create(
         "TestAssembly",
         [generatedTree],
         AppDomain.CurrentDomain.GetAssemblies()
                  .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
                  .Select(a => MetadataReference.CreateFromFile(a.Location)),
         new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

      var type = compilation.GetTypeByMetadataName("Test.MyClass");
      type.Should().NotBeNull();
      var generatedField = type!.GetMembers("_generatedField").OfType<IFieldSymbol>().Single();

      var location = generatedField.GetFieldLocation(CancellationToken.None);

      location.Should().Be(Location.None);
   }

   [Fact]
   public void Should_skip_generated_tree_with_Designer_extension()
   {
      var userSource = @"
namespace Test;

public partial class MyClass
{
   private int _userField;
}
";
      var designerSource = @"
namespace Test;

public partial class MyClass
{
   private int _designerField;
}
";
      var userTree = CSharpSyntaxTree.ParseText(userSource, path: "MyClass.cs", cancellationToken: TestContext.Current.CancellationToken);
      var designerTree = CSharpSyntaxTree.ParseText(designerSource, path: "MyClass.Designer.cs", cancellationToken: TestContext.Current.CancellationToken);

      var compilation = CSharpCompilation.Create(
         "TestAssembly",
         [userTree, designerTree],
         AppDomain.CurrentDomain.GetAssemblies()
                  .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
                  .Select(a => MetadataReference.CreateFromFile(a.Location)),
         new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

      var type = compilation.GetTypeByMetadataName("Test.MyClass");
      type.Should().NotBeNull();
      var userField = type!.GetMembers("_userField").OfType<IFieldSymbol>().Single();

      var location = userField.GetFieldLocation(CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
      location.SourceTree?.FilePath.Should().Be("MyClass.cs");
      location.SourceTree?.FilePath.Should().NotContain(".Designer.cs");
   }

   [Fact]
   public void Should_skip_generated_tree_with_generated_extension()
   {
      var userSource = @"
namespace Test;

public partial class MyClass
{
   private int _userField;
}
";
      var generatedSource = @"
namespace Test;

public partial class MyClass
{
   private int _generatedField;
}
";
      var userTree = CSharpSyntaxTree.ParseText(userSource, path: "MyClass.cs", cancellationToken: TestContext.Current.CancellationToken);
      var generatedTree = CSharpSyntaxTree.ParseText(generatedSource, path: "MyClass.generated.cs", cancellationToken: TestContext.Current.CancellationToken);

      var compilation = CSharpCompilation.Create(
         "TestAssembly",
         [userTree, generatedTree],
         AppDomain.CurrentDomain.GetAssemblies()
                  .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
                  .Select(a => MetadataReference.CreateFromFile(a.Location)),
         new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

      var type = compilation.GetTypeByMetadataName("Test.MyClass");
      type.Should().NotBeNull();
      var userField = type!.GetMembers("_userField").OfType<IFieldSymbol>().Single();

      var location = userField.GetFieldLocation(CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
      location.SourceTree?.FilePath.Should().Be("MyClass.cs");
      location.SourceTree?.FilePath.Should().NotContain(".generated.cs");
   }

   [Fact]
   public void Should_skip_generated_tree_with_g_i_cs_extension()
   {
      var userSource = @"
namespace Test;

public partial class MyClass
{
   private int _userField;
}
";
      var generatedSource = @"
namespace Test;

public partial class MyClass
{
   private int _generatedField;
}
";
      var userTree = CSharpSyntaxTree.ParseText(userSource, path: "MyClass.cs", cancellationToken: TestContext.Current.CancellationToken);
      var generatedTree = CSharpSyntaxTree.ParseText(generatedSource, path: "MyClass.g.i.cs", cancellationToken: TestContext.Current.CancellationToken);

      var compilation = CSharpCompilation.Create(
         "TestAssembly",
         [userTree, generatedTree],
         AppDomain.CurrentDomain.GetAssemblies()
                  .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
                  .Select(a => MetadataReference.CreateFromFile(a.Location)),
         new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

      var type = compilation.GetTypeByMetadataName("Test.MyClass");
      type.Should().NotBeNull();
      var userField = type!.GetMembers("_userField").OfType<IFieldSymbol>().Single();

      var location = userField.GetFieldLocation(CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
      location.SourceTree?.FilePath.Should().Be("MyClass.cs");
      location.SourceTree?.FilePath.Should().NotContain(".g.i.cs");
   }

   [Fact]
   public void Should_handle_field_in_nested_class()
   {
      var src = @"
namespace Test;

public class OuterClass
{
   public class InnerClass
   {
      private int _nestedField;
   }
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.OuterClass+InnerClass");
      var field = type.GetMembers("_nestedField").OfType<IFieldSymbol>().Single();

      var location = field.GetFieldLocation(CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_field_in_generic_class()
   {
      var src = @"
namespace Test;

public class GenericClass<T>
{
   private T _genericField;
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.GenericClass`1");
      var field = type.GetMembers("_genericField").OfType<IFieldSymbol>().Single();

      var location = field.GetFieldLocation(CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_field_in_struct()
   {
      var src = @"
namespace Test;

public struct MyStruct
{
   private int _structField;
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyStruct");
      var field = type.GetMembers("_structField").OfType<IFieldSymbol>().Single();

      var location = field.GetFieldLocation(CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_field_in_record()
   {
      var src = @"
namespace Test;

public record MyRecord
{
   private int _recordField;
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyRecord");
      var field = type.GetMembers("_recordField").OfType<IFieldSymbol>().Single();

      var location = field.GetFieldLocation(CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_field_with_nullable_type()
   {
      var src = @"
#nullable enable
namespace Test;

public class MyClass
{
   private string? _nullableField;
}
";
      var compilation = CreateCompilation(src, nullableContextOptions: NullableContextOptions.Enable);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var field = type.GetMembers("_nullableField").OfType<IFieldSymbol>().Single();

      var location = field.GetFieldLocation(CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_field_with_attributes()
   {
      var src = @"
using System;
using System.ComponentModel;

namespace Test;

public class MyClass
{
   [Obsolete]
   [Description(""Test field"")]
   private int _attributedField;
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var field = type.GetMembers("_attributedField").OfType<IFieldSymbol>().Single();

      var location = field.GetFieldLocation(CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
      var sourceText = location.SourceTree?.GetText(TestContext.Current.CancellationToken).ToString();
      var locationText = sourceText?.Substring(location.SourceSpan.Start, location.SourceSpan.Length);
      locationText.Should().Be("_attributedField");
   }

   [Fact]
   public void Should_handle_field_with_array_type()
   {
      var src = @"
namespace Test;

public class MyClass
{
   private int[] _arrayField;
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var field = type.GetMembers("_arrayField").OfType<IFieldSymbol>().Single();

      var location = field.GetFieldLocation(CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_case_sensitive_field_name_matching_in_multiple_declarators()
   {
      var src = @"
namespace Test;

public class MyClass
{
   private int myfield, MyField, MYFIELD;
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");

      var lowercaseField = type.GetMembers("myfield").OfType<IFieldSymbol>().Single();
      var mixedcaseField = type.GetMembers("MyField").OfType<IFieldSymbol>().Single();
      var uppercaseField = type.GetMembers("MYFIELD").OfType<IFieldSymbol>().Single();

      var lowercaseLocation = lowercaseField.GetFieldLocation(CancellationToken.None);
      var mixedcaseLocation = mixedcaseField.GetFieldLocation(CancellationToken.None);
      var uppercaseLocation = uppercaseField.GetFieldLocation(CancellationToken.None);

      // Verify each gets the correct location (case-sensitive)
      var sourceText = lowercaseLocation.SourceTree?.GetText(TestContext.Current.CancellationToken).ToString();
      sourceText?.Substring(lowercaseLocation.SourceSpan.Start, lowercaseLocation.SourceSpan.Length).Should().Be("myfield");
      sourceText?.Substring(mixedcaseLocation.SourceSpan.Start, mixedcaseLocation.SourceSpan.Length).Should().Be("MyField");
      sourceText?.Substring(uppercaseLocation.SourceSpan.Start, uppercaseLocation.SourceSpan.Length).Should().Be("MYFIELD");
   }

   [Fact]
   public void Should_handle_FieldDeclarationSyntax_with_matching_declarator()
   {
      // This tests the edge case where the syntax reference points to FieldDeclarationSyntax
      // instead of VariableDeclaratorSyntax (rare, but possible)
      var src = @"
namespace Test;

public class MyClass
{
   private int _field1, _field2;
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var field1 = type.GetMembers("_field1").OfType<IFieldSymbol>().Single();
      var field2 = type.GetMembers("_field2").OfType<IFieldSymbol>().Single();

      var location1 = field1.GetFieldLocation(CancellationToken.None);
      var location2 = field2.GetFieldLocation(CancellationToken.None);

      location1.Should().NotBe(Location.None);
      location2.Should().NotBe(Location.None);
      location1.IsInSource.Should().BeTrue();
      location2.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_new_field_hiding_base_field()
   {
      var src = @"
namespace Test;

public class BaseClass
{
   protected int _field;
}

public class DerivedClass : BaseClass
{
   private new int _field;
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.DerivedClass");
      var field = type.GetMembers("_field").OfType<IFieldSymbol>().First();

      var location = field.GetFieldLocation(CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_field_with_complex_generic_type()
   {
      var src = @"
using System.Collections.Generic;

namespace Test;

public class MyClass
{
   private Dictionary<string, List<int>> _complexField;
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var field = type.GetMembers("_complexField").OfType<IFieldSymbol>().Single();

      var location = field.GetFieldLocation(CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_field_in_multiple_partial_class_declarations()
   {
      var part1Source = @"
namespace Test;

public partial class MyClass
{
   private int _field1;
}
";
      var part2Source = @"
namespace Test;

public partial class MyClass
{
   private int _field2;
}
";
      var tree1 = CSharpSyntaxTree.ParseText(part1Source, path: "MyClass.Part1.cs", cancellationToken: TestContext.Current.CancellationToken);
      var tree2 = CSharpSyntaxTree.ParseText(part2Source, path: "MyClass.Part2.cs", cancellationToken: TestContext.Current.CancellationToken);

      var compilation = CSharpCompilation.Create(
         "TestAssembly",
         [tree1, tree2],
         AppDomain.CurrentDomain.GetAssemblies()
                  .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
                  .Select(a => MetadataReference.CreateFromFile(a.Location)),
         new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

      var type = compilation.GetTypeByMetadataName("Test.MyClass");
      type.Should().NotBeNull();

      var field1 = type!.GetMembers("_field1").OfType<IFieldSymbol>().Single();
      var field2 = type.GetMembers("_field2").OfType<IFieldSymbol>().Single();

      var location1 = field1.GetFieldLocation(CancellationToken.None);
      var location2 = field2.GetFieldLocation(CancellationToken.None);

      location1.Should().NotBe(Location.None);
      location2.Should().NotBe(Location.None);
      location1.SourceTree?.FilePath.Should().Be("MyClass.Part1.cs");
      location2.SourceTree?.FilePath.Should().Be("MyClass.Part2.cs");
   }

   [Fact]
   public void Should_handle_static_readonly_field()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public static readonly int StaticReadonlyField = 42;
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var field = type.GetMembers("StaticReadonlyField").OfType<IFieldSymbol>().Single();

      var location = field.GetFieldLocation(CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_field_with_ref_type()
   {
      var src = @"
namespace Test;

public ref struct MyRefStruct
{
   private int _value;
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyRefStruct");
      var field = type.GetMembers("_value").OfType<IFieldSymbol>().Single();

      var location = field.GetFieldLocation(CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_prefer_non_generated_location_when_field_has_multiple_syntax_references()
   {
      var userSource = @"
namespace Test;

public partial class MyClass
{
   private int _sharedField;
}
";
      var generatedSource = @"
namespace Test;

public partial class MyClass
{
   private int _otherField;
}
";
      var userTree = CSharpSyntaxTree.ParseText(userSource, path: "MyClass.cs", cancellationToken: TestContext.Current.CancellationToken);
      var generatedTree = CSharpSyntaxTree.ParseText(generatedSource, path: "MyClass.g.cs", cancellationToken: TestContext.Current.CancellationToken);

      var compilation = CSharpCompilation.Create(
         "TestAssembly",
         [userTree, generatedTree],
         AppDomain.CurrentDomain.GetAssemblies()
                  .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
                  .Select(a => MetadataReference.CreateFromFile(a.Location)),
         new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

      var type = compilation.GetTypeByMetadataName("Test.MyClass");
      type.Should().NotBeNull();
      var sharedField = type!.GetMembers("_sharedField").OfType<IFieldSymbol>().Single();

      var location = sharedField.GetFieldLocation(CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
      location.SourceTree?.FilePath.Should().Be("MyClass.cs");
   }

   [Fact]
   public void Should_handle_unsafe_field()
   {
      var src = @"
namespace Test;

public unsafe class MyClass
{
   private int* _unsafeField;
}
";
      var compilation = CreateCompilation(src, allowUnsafe: true);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var field = type.GetMembers("_unsafeField").OfType<IFieldSymbol>().Single();

      var location = field.GetFieldLocation(CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_fixed_size_buffer_field()
   {
      var src = @"
namespace Test;

public unsafe struct MyStruct
{
   public fixed byte Buffer[16];
}
";
      var compilation = CreateCompilation(src, allowUnsafe: true);
      var type = GetTypeSymbol(compilation, "Test.MyStruct");
      var field = type.GetMembers("Buffer").OfType<IFieldSymbol>().Single();

      var location = field.GetFieldLocation(CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }
}

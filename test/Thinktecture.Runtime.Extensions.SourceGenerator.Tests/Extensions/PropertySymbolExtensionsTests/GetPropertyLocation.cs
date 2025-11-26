using System;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Thinktecture.CodeAnalysis;

namespace Thinktecture.Runtime.Tests.PropertySymbolExtensionsTests;

public class GetPropertyLocation : CompilationTestBase
{
   [Fact]
   public void Should_return_identifier_location_for_auto_property()
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

      var location = property.GetPropertyLocation(PropertyDeclarationSyntaxKind.All, CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
      var sourceText = location.SourceTree?.GetText(TestContext.Current.CancellationToken).ToString();
      var locationText = sourceText?.Substring(location.SourceSpan.Start, location.SourceSpan.Length);
      locationText.Should().Be("Value");
   }

   [Fact]
   public void Should_return_identifier_location_for_get_only_property()
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

      var location = property.GetPropertyLocation(PropertyDeclarationSyntaxKind.All, CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
      var sourceText = location.SourceTree?.GetText(TestContext.Current.CancellationToken).ToString();
      var locationText = sourceText?.Substring(location.SourceSpan.Start, location.SourceSpan.Length);
      locationText.Should().Be("Value");
   }

   [Fact]
   public void Should_return_identifier_location_for_expression_bodied_property()
   {
      var src = @"
namespace Test;

public class MyClass
{
   private int _value;

   public int Value => _value;
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var property = type.GetMembers("Value").OfType<IPropertySymbol>().Single();

      var location = property.GetPropertyLocation(PropertyDeclarationSyntaxKind.All, CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
      var sourceText = location.SourceTree?.GetText(TestContext.Current.CancellationToken).ToString();
      var locationText = sourceText?.Substring(location.SourceSpan.Start, location.SourceSpan.Length);
      locationText.Should().Be("Value");
   }

   [Fact]
   public void Should_return_identifier_location_for_property_with_init_accessor()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public int Value { get; init; }
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var property = type.GetMembers("Value").OfType<IPropertySymbol>().Single();

      var location = property.GetPropertyLocation(PropertyDeclarationSyntaxKind.All, CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
      var sourceText = location.SourceTree?.GetText(TestContext.Current.CancellationToken).ToString();
      var locationText = sourceText?.Substring(location.SourceSpan.Start, location.SourceSpan.Length);
      locationText.Should().Be("Value");
   }

   [Fact]
   public void Should_return_identifier_location_for_property_with_accessor_bodies()
   {
      var src = @"
namespace Test;

public class MyClass
{
   private int _value;

   public int Value
   {
      get { return _value; }
      set { _value = value; }
   }
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var property = type.GetMembers("Value").OfType<IPropertySymbol>().Single();

      var location = property.GetPropertyLocation(PropertyDeclarationSyntaxKind.All, CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
      var sourceText = location.SourceTree?.GetText(TestContext.Current.CancellationToken).ToString();
      var locationText = sourceText?.Substring(location.SourceSpan.Start, location.SourceSpan.Length);
      locationText.Should().Be("Value");
   }

   [Fact]
   public void Should_return_identifier_location_for_property_with_expression_bodied_accessors()
   {
      var src = @"
namespace Test;

public class MyClass
{
   private int _value;

   public int Value
   {
      get => _value;
      set => _value = value;
   }
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var property = type.GetMembers("Value").OfType<IPropertySymbol>().Single();

      var location = property.GetPropertyLocation(PropertyDeclarationSyntaxKind.All, CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
      var sourceText = location.SourceTree?.GetText(TestContext.Current.CancellationToken).ToString();
      var locationText = sourceText?.Substring(location.SourceSpan.Start, location.SourceSpan.Length);
      locationText.Should().Be("Value");
   }

   [Fact]
   public void Should_return_identifier_location_for_partial_property_with_expression_body()
   {
      var src = @"
namespace Test;

public partial class MyClass
{
   private int _value;

   public partial int Value => _value;
}
";
      var compilation = CreateCompilation(src, expectedCompilerErrors: ["Partial property 'MyClass.Value' must have a definition part."]);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var property = type.GetMembers("Value").OfType<IPropertySymbol>().Single();

      var location = property.GetPropertyLocation(PropertyDeclarationSyntaxKind.All, CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_return_identifier_location_for_partial_property_with_accessor_body()
   {
      var src = @"
namespace Test;

public partial class MyClass
{
   private int _value;

   public partial int Value
   {
      get { return _value; }
   }
}
";
      var compilation = CreateCompilation(src, expectedCompilerErrors: ["Partial property 'MyClass.Value' must have a definition part."]);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var property = type.GetMembers("Value").OfType<IPropertySymbol>().Single();

      var location = property.GetPropertyLocation(PropertyDeclarationSyntaxKind.All, CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_return_identifier_location_for_partial_property_declaration_only()
   {
      var src = @"
namespace Test;

public partial class MyClass
{
   public partial int Value { get; }
}
";
      var compilation = CreateCompilation(src, expectedCompilerErrors: ["Partial property 'MyClass.Value' must have an implementation part."]);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var property = type.GetMembers("Value").OfType<IPropertySymbol>().Single();

      var location = property.GetPropertyLocation(PropertyDeclarationSyntaxKind.All, CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_return_location_when_preferredKind_is_Implementation_for_regular_property()
   {
      var src = @"
namespace Test;

public class MyClass
{
   private int _value;

   public int Value
   {
      get { return _value; }
      set { _value = value; }
   }
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var property = type.GetMembers("Value").OfType<IPropertySymbol>().Single();

      var location = property.GetPropertyLocation(PropertyDeclarationSyntaxKind.Implementation, CancellationToken.None);

      // Regular properties (non-partial) return immediately
      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_prefer_declaration_when_preferredKind_is_DeclarationOnly()
   {
      var declarationSource = @"
namespace Test;

public partial class MyClass
{
   public partial int Value { get; }
}
";
      var implementationSource = @"
namespace Test;

public partial class MyClass
{
   private int _value;

   public partial int Value => _value;
}
";
      var declarationTree = CSharpSyntaxTree.ParseText(declarationSource, path: "MyClass.Declaration.cs", cancellationToken: TestContext.Current.CancellationToken);
      var implementationTree = CSharpSyntaxTree.ParseText(implementationSource, path: "MyClass.Implementation.cs", cancellationToken: TestContext.Current.CancellationToken);

      var compilation = CSharpCompilation.Create(
         "TestAssembly",
         [declarationTree, implementationTree],
         AppDomain.CurrentDomain.GetAssemblies()
                  .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
                  .Select(a => MetadataReference.CreateFromFile(a.Location)),
         new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

      var type = compilation.GetTypeByMetadataName("Test.MyClass");
      type.Should().NotBeNull();
      var property = type!.GetMembers("Value").OfType<IPropertySymbol>().Single();

      var location = property.GetPropertyLocation(PropertyDeclarationSyntaxKind.DeclarationOnly, CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.SourceTree?.FilePath.Should().Be("MyClass.Declaration.cs");
   }

   [Fact]
   public void Should_return_implementation_when_preferredKind_is_Implementation_but_only_implementation_exists()
   {
      var src = @"
namespace Test;

public partial class MyClass
{
   private int _value;

   public partial int Value => _value;
}
";
      var compilation = CreateCompilation(src, expectedCompilerErrors: ["Partial property 'MyClass.Value' must have a definition part."]);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var property = type.GetMembers("Value").OfType<IPropertySymbol>().Single();

      var location = property.GetPropertyLocation(PropertyDeclarationSyntaxKind.Implementation, CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_return_declaration_when_preferredKind_is_DeclarationOnly_but_only_declaration_exists()
   {
      var src = @"
namespace Test;

public partial class MyClass
{
   public partial int Value { get; }
}
";
      var compilation = CreateCompilation(src, expectedCompilerErrors: ["Partial property 'MyClass.Value' must have an implementation part."]);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var property = type.GetMembers("Value").OfType<IPropertySymbol>().Single();

      var location = property.GetPropertyLocation(PropertyDeclarationSyntaxKind.DeclarationOnly, CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_return_this_keyword_location_for_indexer()
   {
      var src = @"
namespace Test;

public class MyClass
{
   private int[] _items = new int[10];

   public int this[int index]
   {
      get => _items[index];
      set => _items[index] = value;
   }
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var indexer = type.GetMembers().OfType<IPropertySymbol>().Single(p => p.IsIndexer);

      var location = indexer.GetPropertyLocation(PropertyDeclarationSyntaxKind.All, CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
      var sourceText = location.SourceTree?.GetText(TestContext.Current.CancellationToken).ToString();
      var locationText = sourceText?.Substring(location.SourceSpan.Start, location.SourceSpan.Length);
      locationText.Should().Be("this");
   }

   [Fact]
   public void Should_return_this_keyword_location_for_expression_bodied_indexer()
   {
      var src = @"
namespace Test;

public class MyClass
{
   private int[] _items = new int[10];

   public int this[int index] => _items[index];
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var indexer = type.GetMembers().OfType<IPropertySymbol>().Single(p => p.IsIndexer);

      var location = indexer.GetPropertyLocation(PropertyDeclarationSyntaxKind.All, CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
      var sourceText = location.SourceTree?.GetText(TestContext.Current.CancellationToken).ToString();
      var locationText = sourceText?.Substring(location.SourceSpan.Start, location.SourceSpan.Length);
      locationText.Should().Be("this");
   }

   [Fact]
   public void Should_return_this_keyword_location_for_partial_indexer_with_implementation()
   {
      var src = @"
namespace Test;

public partial class MyClass
{
   private int[] _items = new int[10];

   public partial int this[int index]
   {
      get { return _items[index]; }
   }
}
";
      var compilation = CreateCompilation(src, expectedCompilerErrors: ["Partial property 'MyClass.this[int]' must have a definition part."]);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var indexer = type.GetMembers().OfType<IPropertySymbol>().Single(p => p.IsIndexer);

      var location = indexer.GetPropertyLocation(PropertyDeclarationSyntaxKind.All, CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_return_this_keyword_location_for_partial_indexer_declaration_only()
   {
      var src = @"
namespace Test;

public partial class MyClass
{
   public partial int this[int index] { get; }
}
";
      var compilation = CreateCompilation(src, expectedCompilerErrors: ["Partial property 'MyClass.this[int]' must have an implementation part."]);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var indexer = type.GetMembers().OfType<IPropertySymbol>().Single(p => p.IsIndexer);

      var location = indexer.GetPropertyLocation(PropertyDeclarationSyntaxKind.All, CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_return_this_keyword_location_for_indexer_with_multiple_parameters()
   {
      var src = @"
namespace Test;

public class MyClass
{
   private int[,] _matrix = new int[10, 10];

   public int this[int row, int col]
   {
      get => _matrix[row, col];
      set => _matrix[row, col] = value;
   }
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var indexer = type.GetMembers().OfType<IPropertySymbol>().Single(p => p.IsIndexer);

      var location = indexer.GetPropertyLocation(PropertyDeclarationSyntaxKind.All, CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
      var sourceText = location.SourceTree?.GetText(TestContext.Current.CancellationToken).ToString();
      var locationText = sourceText?.Substring(location.SourceSpan.Start, location.SourceSpan.Length);
      locationText.Should().Be("this");
   }

   [Fact]
   public void Should_return_parameter_identifier_location_for_record_positional_property()
   {
      var src = @"
namespace Test;

public record Person(string Name, int Age);
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.Person");
      var nameProperty = type.GetMembers("Name").OfType<IPropertySymbol>().Single();

      var location = nameProperty.GetPropertyLocation(PropertyDeclarationSyntaxKind.All, CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
      var sourceText = location.SourceTree?.GetText(TestContext.Current.CancellationToken).ToString();
      var locationText = sourceText?.Substring(location.SourceSpan.Start, location.SourceSpan.Length);
      locationText.Should().Be("Name");
   }

   [Fact]
   public void Should_return_parameter_identifier_location_for_record_struct_positional_property()
   {
      var src = @"
namespace Test;

public record struct Point(int X, int Y);
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.Point");
      var xProperty = type.GetMembers("X").OfType<IPropertySymbol>().Single();

      var location = xProperty.GetPropertyLocation(PropertyDeclarationSyntaxKind.All, CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
      var sourceText = location.SourceTree?.GetText(TestContext.Current.CancellationToken).ToString();
      var locationText = sourceText?.Substring(location.SourceSpan.Start, location.SourceSpan.Length);
      locationText.Should().Be("X");
   }

   [Fact]
   public void Should_return_parameter_identifier_location_for_second_record_positional_property()
   {
      var src = @"
namespace Test;

public record Person(string Name, int Age);
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.Person");
      var ageProperty = type.GetMembers("Age").OfType<IPropertySymbol>().Single();

      var location = ageProperty.GetPropertyLocation(PropertyDeclarationSyntaxKind.All, CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
      var sourceText = location.SourceTree?.GetText(TestContext.Current.CancellationToken).ToString();
      var locationText = sourceText?.Substring(location.SourceSpan.Start, location.SourceSpan.Length);
      locationText.Should().Be("Age");
   }

   [Fact]
   public void Should_skip_generated_tree_and_return_non_generated_location()
   {
      var userSource = @"
namespace Test;

public partial class MyClass
{
   public int UserProperty { get; set; }
}
";
      var generatedSource = @"
namespace Test;

public partial class MyClass
{
   public int GeneratedProperty { get; set; }
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
      var userProperty = type!.GetMembers("UserProperty").OfType<IPropertySymbol>().Single();

      var location = userProperty.GetPropertyLocation(PropertyDeclarationSyntaxKind.All, CancellationToken.None);

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
   public int GeneratedProperty { get; set; }
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
      var generatedProperty = type!.GetMembers("GeneratedProperty").OfType<IPropertySymbol>().Single();

      var location = generatedProperty.GetPropertyLocation(PropertyDeclarationSyntaxKind.All, CancellationToken.None);

      location.Should().Be(Location.None);
   }

   [Fact]
   public void Should_skip_generated_tree_with_Designer_extension()
   {
      var userSource = @"
namespace Test;

public partial class MyClass
{
   public int UserProperty { get; set; }
}
";
      var designerSource = @"
namespace Test;

public partial class MyClass
{
   public int DesignerProperty { get; set; }
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
      var userProperty = type!.GetMembers("UserProperty").OfType<IPropertySymbol>().Single();

      var location = userProperty.GetPropertyLocation(PropertyDeclarationSyntaxKind.All, CancellationToken.None);

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
   public int UserProperty { get; set; }
}
";
      var generatedSource = @"
namespace Test;

public partial class MyClass
{
   public int GeneratedProperty { get; set; }
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
      var userProperty = type!.GetMembers("UserProperty").OfType<IPropertySymbol>().Single();

      var location = userProperty.GetPropertyLocation(PropertyDeclarationSyntaxKind.All, CancellationToken.None);

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
   public int UserProperty { get; set; }
}
";
      var generatedSource = @"
namespace Test;

public partial class MyClass
{
   public int GeneratedProperty { get; set; }
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
      var userProperty = type!.GetMembers("UserProperty").OfType<IPropertySymbol>().Single();

      var location = userProperty.GetPropertyLocation(PropertyDeclarationSyntaxKind.All, CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
      location.SourceTree?.FilePath.Should().Be("MyClass.cs");
      location.SourceTree?.FilePath.Should().NotContain(".g.i.cs");
   }

   [Fact]
   public void Should_handle_static_property()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public static int StaticProperty { get; set; }
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var property = type.GetMembers("StaticProperty").OfType<IPropertySymbol>().Single();

      var location = property.GetPropertyLocation(PropertyDeclarationSyntaxKind.All, CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_private_property()
   {
      var src = @"
namespace Test;

public class MyClass
{
   private int PrivateProperty { get; set; }
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var property = type.GetMembers("PrivateProperty").OfType<IPropertySymbol>().Single();

      var location = property.GetPropertyLocation(PropertyDeclarationSyntaxKind.All, CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_protected_property()
   {
      var src = @"
namespace Test;

public class MyClass
{
   protected int ProtectedProperty { get; set; }
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var property = type.GetMembers("ProtectedProperty").OfType<IPropertySymbol>().Single();

      var location = property.GetPropertyLocation(PropertyDeclarationSyntaxKind.All, CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_internal_property()
   {
      var src = @"
namespace Test;

public class MyClass
{
   internal int InternalProperty { get; set; }
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var property = type.GetMembers("InternalProperty").OfType<IPropertySymbol>().Single();

      var location = property.GetPropertyLocation(PropertyDeclarationSyntaxKind.All, CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_protected_internal_property()
   {
      var src = @"
namespace Test;

public class MyClass
{
   protected internal int ProtectedInternalProperty { get; set; }
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var property = type.GetMembers("ProtectedInternalProperty").OfType<IPropertySymbol>().Single();

      var location = property.GetPropertyLocation(PropertyDeclarationSyntaxKind.All, CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_private_protected_property()
   {
      var src = @"
namespace Test;

public class MyClass
{
   private protected int PrivateProtectedProperty { get; set; }
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var property = type.GetMembers("PrivateProtectedProperty").OfType<IPropertySymbol>().Single();

      var location = property.GetPropertyLocation(PropertyDeclarationSyntaxKind.All, CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_virtual_property()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public virtual int VirtualProperty { get; set; }
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var property = type.GetMembers("VirtualProperty").OfType<IPropertySymbol>().Single();

      var location = property.GetPropertyLocation(PropertyDeclarationSyntaxKind.All, CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_abstract_property()
   {
      var src = @"
namespace Test;

public abstract class MyClass
{
   public abstract int AbstractProperty { get; set; }
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var property = type.GetMembers("AbstractProperty").OfType<IPropertySymbol>().Single();

      var location = property.GetPropertyLocation(PropertyDeclarationSyntaxKind.All, CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_override_property()
   {
      var src = @"
namespace Test;

public class BaseClass
{
   public virtual int VirtualProperty { get; set; }
}

public class DerivedClass : BaseClass
{
   public override int VirtualProperty { get; set; }
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.DerivedClass");
      var property = type.GetMembers("VirtualProperty").OfType<IPropertySymbol>().Single();

      var location = property.GetPropertyLocation(PropertyDeclarationSyntaxKind.All, CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_property_in_interface()
   {
      var src = @"
namespace Test;

public interface IMyInterface
{
   int InterfaceProperty { get; set; }
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.IMyInterface");
      var property = type.GetMembers("InterfaceProperty").OfType<IPropertySymbol>().Single();

      var location = property.GetPropertyLocation(PropertyDeclarationSyntaxKind.All, CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_explicit_interface_implementation()
   {
      var src = @"
namespace Test;

public interface IMyInterface
{
   int InterfaceProperty { get; set; }
}

public class MyClass : IMyInterface
{
   int IMyInterface.InterfaceProperty { get; set; }
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var property = type.GetMembers("Test.IMyInterface.InterfaceProperty").OfType<IPropertySymbol>().Single();

      var location = property.GetPropertyLocation(PropertyDeclarationSyntaxKind.All, CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_property_in_nested_class()
   {
      var src = @"
namespace Test;

public class OuterClass
{
   public class InnerClass
   {
      public int NestedProperty { get; set; }
   }
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.OuterClass+InnerClass");
      var property = type.GetMembers("NestedProperty").OfType<IPropertySymbol>().Single();

      var location = property.GetPropertyLocation(PropertyDeclarationSyntaxKind.All, CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_property_in_generic_class()
   {
      var src = @"
namespace Test;

public class GenericClass<T>
{
   public T GenericProperty { get; set; }
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.GenericClass`1");
      var property = type.GetMembers("GenericProperty").OfType<IPropertySymbol>().Single();

      var location = property.GetPropertyLocation(PropertyDeclarationSyntaxKind.All, CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_property_in_struct()
   {
      var src = @"
namespace Test;

public struct MyStruct
{
   public int StructProperty { get; set; }
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyStruct");
      var property = type.GetMembers("StructProperty").OfType<IPropertySymbol>().Single();

      var location = property.GetPropertyLocation(PropertyDeclarationSyntaxKind.All, CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_property_in_record()
   {
      var src = @"
namespace Test;

public record MyRecord
{
   public int RecordProperty { get; set; }
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyRecord");
      var property = type.GetMembers("RecordProperty").OfType<IPropertySymbol>().Single();

      var location = property.GetPropertyLocation(PropertyDeclarationSyntaxKind.All, CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_required_property()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public required int RequiredProperty { get; set; }
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var property = type.GetMembers("RequiredProperty").OfType<IPropertySymbol>().Single();

      var location = property.GetPropertyLocation(PropertyDeclarationSyntaxKind.All, CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_property_with_nullable_type()
   {
      var src = @"
#nullable enable
namespace Test;

public class MyClass
{
   public string? NullableProperty { get; set; }
}
";
      var compilation = CreateCompilation(src, nullableContextOptions: NullableContextOptions.Enable);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var property = type.GetMembers("NullableProperty").OfType<IPropertySymbol>().Single();

      var location = property.GetPropertyLocation(PropertyDeclarationSyntaxKind.All, CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_property_with_attributes()
   {
      var src = @"
using System;
using System.ComponentModel;

namespace Test;

public class MyClass
{
   [Obsolete]
   [Description(""Test property"")]
   public int AttributedProperty { get; set; }
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var property = type.GetMembers("AttributedProperty").OfType<IPropertySymbol>().Single();

      var location = property.GetPropertyLocation(PropertyDeclarationSyntaxKind.All, CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
      var sourceText = location.SourceTree?.GetText(TestContext.Current.CancellationToken).ToString();
      var locationText = sourceText?.Substring(location.SourceSpan.Start, location.SourceSpan.Length);
      locationText.Should().Be("AttributedProperty");
   }

   [Fact]
   public void Should_handle_sealed_override_property()
   {
      var src = @"
namespace Test;

public class BaseClass
{
   public virtual int VirtualProperty { get; set; }
}

public class DerivedClass : BaseClass
{
   public sealed override int VirtualProperty { get; set; }
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.DerivedClass");
      var property = type.GetMembers("VirtualProperty").OfType<IPropertySymbol>().Single();

      var location = property.GetPropertyLocation(PropertyDeclarationSyntaxKind.All, CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_new_property_hiding_base_property()
   {
      var src = @"
namespace Test;

public class BaseClass
{
   public int HiddenProperty { get; set; }
}

public class DerivedClass : BaseClass
{
   public new int HiddenProperty { get; set; }
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.DerivedClass");
      var property = type.GetMembers("HiddenProperty").OfType<IPropertySymbol>().First();

      var location = property.GetPropertyLocation(PropertyDeclarationSyntaxKind.All, CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_return_location_when_preferredKind_is_All_for_regular_property()
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

      var location = property.GetPropertyLocation(PropertyDeclarationSyntaxKind.All, CancellationToken.None);

      // Regular properties (non-partial) return immediately regardless of preferred kind
      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_return_fallback_location_when_only_implementation_exists_and_DeclarationOnly_preferred()
   {
      var src = @"
namespace Test;

public partial class MyClass
{
   private int _value;

   public partial int Value => _value;
}
";
      var compilation = CreateCompilation(src, expectedCompilerErrors: ["Partial property 'MyClass.Value' must have a definition part."]);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var property = type.GetMembers("Value").OfType<IPropertySymbol>().Single();

      var location = property.GetPropertyLocation(PropertyDeclarationSyntaxKind.DeclarationOnly, CancellationToken.None);

      // Should still return a location (the implementation) as fallback
      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_return_fallback_location_when_only_declaration_exists_and_Implementation_preferred()
   {
      var src = @"
namespace Test;

public partial class MyClass
{
   public partial int Value { get; }
}
";
      var compilation = CreateCompilation(src, expectedCompilerErrors: ["Partial property 'MyClass.Value' must have an implementation part."]);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var property = type.GetMembers("Value").OfType<IPropertySymbol>().Single();

      var location = property.GetPropertyLocation(PropertyDeclarationSyntaxKind.Implementation, CancellationToken.None);

      // Should still return a location (the declaration) as fallback
      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_property_with_mixed_accessor_implementations()
   {
      var src = @"
namespace Test;

public partial class MyClass
{
   private int _value;

   public partial int Value
   {
      get { return _value; }
      set => _value = value;
   }
}
";
      var compilation = CreateCompilation(src, expectedCompilerErrors: ["Partial property 'MyClass.Value' must have a definition part."]);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var property = type.GetMembers("Value").OfType<IPropertySymbol>().Single();

      var location = property.GetPropertyLocation(PropertyDeclarationSyntaxKind.All, CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_partial_property_with_only_get_accessor_body()
   {
      var src = @"
namespace Test;

public partial class MyClass
{
   private int _value;

   public partial int Value
   {
      get => _value;
   }
}
";
      var compilation = CreateCompilation(src, expectedCompilerErrors: ["Partial property 'MyClass.Value' must have a definition part."]);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var property = type.GetMembers("Value").OfType<IPropertySymbol>().Single();

      var location = property.GetPropertyLocation(PropertyDeclarationSyntaxKind.All, CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_readonly_struct_property()
   {
      var src = @"
namespace Test;

public readonly struct ReadOnlyStruct
{
   public int ReadOnlyProperty { get; }

   public ReadOnlyStruct(int value)
   {
      ReadOnlyProperty = value;
   }
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.ReadOnlyStruct");
      var property = type.GetMembers("ReadOnlyProperty").OfType<IPropertySymbol>().Single();

      var location = property.GetPropertyLocation(PropertyDeclarationSyntaxKind.All, CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_ref_readonly_property()
   {
      var src = @"
namespace Test;

public class MyClass
{
   private int _value;

   public ref readonly int RefReadOnlyProperty => ref _value;
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var property = type.GetMembers("RefReadOnlyProperty").OfType<IPropertySymbol>().Single();

      var location = property.GetPropertyLocation(PropertyDeclarationSyntaxKind.All, CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_partial_class_with_property_across_multiple_files()
   {
      var partialClass1Source = @"
namespace Test;

public partial class MyClass
{
   public int OtherProperty { get; set; }
}
";
      var partialClass2Source = @"
namespace Test;

public partial class MyClass
{
   private int _value;
}
";
      var partialClass3Source = @"
namespace Test;

public partial class MyClass
{
   public partial int Value => _value;
}
";
      var tree1 = CSharpSyntaxTree.ParseText(partialClass1Source, path: "AMyClass.Part1.cs", cancellationToken: TestContext.Current.CancellationToken);
      var tree2 = CSharpSyntaxTree.ParseText(partialClass2Source, path: "BMyClass.Part2.cs", cancellationToken: TestContext.Current.CancellationToken);
      var tree3 = CSharpSyntaxTree.ParseText(partialClass3Source, path: "ZMyClass.Part3.cs", cancellationToken: TestContext.Current.CancellationToken);

      var compilation = CSharpCompilation.Create(
         "TestAssembly",
         [tree1, tree2, tree3],
         AppDomain.CurrentDomain.GetAssemblies()
                  .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
                  .Select(a => MetadataReference.CreateFromFile(a.Location)),
         new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

      var type = compilation.GetTypeByMetadataName("Test.MyClass");
      type.Should().NotBeNull();

      // Get the Value property which is declared in one of the partial class parts
      var property = type!.GetMembers("Value").OfType<IPropertySymbol>().First();

      var location = property.GetPropertyLocation(PropertyDeclarationSyntaxKind.All, CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
      location.SourceTree?.FilePath.Should().Be("ZMyClass.Part3.cs");
   }
}

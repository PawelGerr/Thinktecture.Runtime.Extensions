using System;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture.Runtime.Tests.NamedTypeSymbolExtensionsTests;

public class GetTypeIdentifierLocation : CompilationTestBase
{
   [Fact]
   public void Should_return_identifier_location_for_class()
   {
      var src = @"
namespace Test;

public class MyClass
{
}
";
      var type = GetTypeSymbol(src, "Test.MyClass");

      var result = type.GetTypeIdentifierLocation(CancellationToken.None);

      result.Should().NotBe(Location.None);
      result.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_return_identifier_location_for_struct()
   {
      var src = @"
namespace Test;

public struct MyStruct
{
}
";
      var type = GetTypeSymbol(src, "Test.MyStruct");

      var result = type.GetTypeIdentifierLocation(CancellationToken.None);

      result.Should().NotBe(Location.None);
      result.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_return_identifier_location_for_record()
   {
      var src = @"
namespace Test;

public record MyRecord(string Name);
";
      var type = GetTypeSymbol(src, "Test.MyRecord");

      var result = type.GetTypeIdentifierLocation(CancellationToken.None);

      result.Should().NotBe(Location.None);
      result.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_return_identifier_location_for_record_struct()
   {
      var src = @"
namespace Test;

public record struct MyRecordStruct(int Value);
";
      var type = GetTypeSymbol(src, "Test.MyRecordStruct");

      var result = type.GetTypeIdentifierLocation(CancellationToken.None);

      result.Should().NotBe(Location.None);
      result.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_return_identifier_location_for_interface()
   {
      var src = @"
namespace Test;

public interface IMyInterface
{
}
";
      var type = GetTypeSymbol(src, "Test.IMyInterface");

      var result = type.GetTypeIdentifierLocation(CancellationToken.None);

      result.Should().NotBe(Location.None);
      result.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_return_identifier_location_for_enum()
   {
      var src = @"
namespace Test;

public enum MyEnum
{
   Value1,
   Value2
}
";
      var type = GetTypeSymbol(src, "Test.MyEnum");

      var result = type.GetTypeIdentifierLocation(CancellationToken.None);

      result.Should().NotBe(Location.None);
      result.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_return_identifier_location_for_nested_class()
   {
      var src = @"
namespace Test;

public class Outer
{
   public class Inner
   {
   }
}
";
      var type = GetTypeSymbol(src, "Test.Outer+Inner");

      var result = type.GetTypeIdentifierLocation(CancellationToken.None);

      result.Should().NotBe(Location.None);
      result.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_return_identifier_location_for_generic_class()
   {
      var src = @"
namespace Test;

public class GenericClass<T>
{
}
";
      var type = GetTypeSymbol(src, "Test.GenericClass`1");

      var result = type.GetTypeIdentifierLocation(CancellationToken.None);

      result.Should().NotBe(Location.None);
      result.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_return_identifier_location_for_partial_class()
   {
      var src = @"
namespace Test;

public partial class PartialClass
{
}
";
      var type = GetTypeSymbol(src, "Test.PartialClass");

      var result = type.GetTypeIdentifierLocation(CancellationToken.None);

      result.Should().NotBe(Location.None);
      result.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_return_first_non_generated_location_when_multiple_partial_declarations()
   {
      var src1 = @"
namespace Test;

public partial class MultiPartialClass
{
}
";
      var src2 = @"
namespace Test;

public partial class MultiPartialClass
{
   public void Method1() { }
}
";
      var syntaxTree1 = CSharpSyntaxTree.ParseText(src1, CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Preview));
      var syntaxTree2 = CSharpSyntaxTree.ParseText(src2, CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Preview));

      var references = AppDomain.CurrentDomain.GetAssemblies()
                                .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
                                .Select(a => MetadataReference.CreateFromFile(a.Location));

      var compilation = CSharpCompilation.Create(
         "MultiPartialTest",
         [syntaxTree1, syntaxTree2],
         references,
         new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

      var type = compilation.GetTypeByMetadataName("Test.MultiPartialClass");
      type.Should().NotBeNull();

      var result = type!.GetTypeIdentifierLocation(CancellationToken.None);

      result.Should().NotBe(Location.None);
      result.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_return_location_none_for_type_without_syntax_references()
   {
      // System types don't have syntax references in user code
      var src = @"
namespace Test;

public class MyClass
{
   public System.String Field;
}
";
      var compilation = CreateCompilation(src);
      var stringType = compilation.GetSpecialType(SpecialType.System_String);

      var result = stringType.GetTypeIdentifierLocation(CancellationToken.None);

      result.Should().Be(Location.None);
   }

   [Fact]
   public void Should_prefer_type_declaration_syntax_over_other_syntax_nodes()
   {
      var src = @"
namespace Test;

public class MyClass
{
}
";
      var type = GetTypeSymbol(src, "Test.MyClass");

      var result = type.GetTypeIdentifierLocation(CancellationToken.None);

      result.Should().NotBe(Location.None);
      result.IsInSource.Should().BeTrue();

      // Verify the location points to the identifier specifically
      var node = type.DeclaringSyntaxReferences[0].GetSyntax(CancellationToken.None);
      var tds = node.Should().BeOfType<ClassDeclarationSyntax>().Subject;
      var identifierLocation = tds.Identifier.GetLocation();
      result.Should().Be(identifierLocation);
   }

   [Fact]
   public void Should_handle_abstract_class()
   {
      var src = @"
namespace Test;

public abstract class AbstractClass
{
}
";
      var type = GetTypeSymbol(src, "Test.AbstractClass");

      var result = type.GetTypeIdentifierLocation(CancellationToken.None);

      result.Should().NotBe(Location.None);
      result.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_sealed_class()
   {
      var src = @"
namespace Test;

public sealed class SealedClass
{
}
";
      var type = GetTypeSymbol(src, "Test.SealedClass");

      var result = type.GetTypeIdentifierLocation(CancellationToken.None);

      result.Should().NotBe(Location.None);
      result.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_static_class()
   {
      var src = @"
namespace Test;

public static class StaticClass
{
}
";
      var type = GetTypeSymbol(src, "Test.StaticClass");

      var result = type.GetTypeIdentifierLocation(CancellationToken.None);

      result.Should().NotBe(Location.None);
      result.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_internal_class()
   {
      var src = @"
namespace Test;

internal class InternalClass
{
}
";
      var type = GetTypeSymbol(src, "Test.InternalClass");

      var result = type.GetTypeIdentifierLocation(CancellationToken.None);

      result.Should().NotBe(Location.None);
      result.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_deeply_nested_class()
   {
      var src = @"
namespace Test;

public class Level1
{
   public class Level2
   {
      public class Level3
      {
         public class Level4
         {
         }
      }
   }
}
";
      var type = GetTypeSymbol(src, "Test.Level1+Level2+Level3+Level4");

      var result = type.GetTypeIdentifierLocation(CancellationToken.None);

      result.Should().NotBe(Location.None);
      result.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_class_with_constraints()
   {
      var src = @"
namespace Test;

public class GenericClass<T> where T : class
{
}
";
      var type = GetTypeSymbol(src, "Test.GenericClass`1");

      var result = type.GetTypeIdentifierLocation(CancellationToken.None);

      result.Should().NotBe(Location.None);
      result.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_class_with_multiple_type_parameters()
   {
      var src = @"
namespace Test;

public class GenericClass<T1, T2, T3>
{
}
";
      var type = GetTypeSymbol(src, "Test.GenericClass`3");

      var result = type.GetTypeIdentifierLocation(CancellationToken.None);

      result.Should().NotBe(Location.None);
      result.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_nested_generic_class()
   {
      var src = @"
namespace Test;

public class Outer<T>
{
   public class Inner<U>
   {
   }
}
";
      var type = GetTypeSymbol(src, "Test.Outer`1+Inner`1");

      var result = type.GetTypeIdentifierLocation(CancellationToken.None);

      result.Should().NotBe(Location.None);
      result.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_record_class()
   {
      var src = @"
namespace Test;

public record class RecordClass(string Name);
";
      var type = GetTypeSymbol(src, "Test.RecordClass");

      var result = type.GetTypeIdentifierLocation(CancellationToken.None);

      result.Should().NotBe(Location.None);
      result.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_readonly_struct()
   {
      var src = @"
namespace Test;

public readonly struct ReadonlyStruct
{
}
";
      var type = GetTypeSymbol(src, "Test.ReadonlyStruct");

      var result = type.GetTypeIdentifierLocation(CancellationToken.None);

      result.Should().NotBe(Location.None);
      result.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_ref_struct()
   {
      var src = @"
namespace Test;

public ref struct RefStruct
{
}
";
      var type = GetTypeSymbol(src, "Test.RefStruct");

      var result = type.GetTypeIdentifierLocation(CancellationToken.None);

      result.Should().NotBe(Location.None);
      result.IsInSource.Should().BeTrue();
   }

   private static INamedTypeSymbol GetTypeSymbol(string source, string metadataName)
   {
      var parseOptions = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Preview);
      var syntaxTree = CSharpSyntaxTree.ParseText(source, parseOptions);
      var references = AppDomain.CurrentDomain.GetAssemblies()
                                .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
                                .Select(a => MetadataReference.CreateFromFile(a.Location));

      var compilation = CSharpCompilation.Create(
         "GetTypeIdentifierLocationTests",
         [syntaxTree],
         references,
         new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Release));

      var type = compilation.GetTypeByMetadataName(metadataName);

      return type ?? throw new InvalidOperationException($"Type '{metadataName}' not found. Diagnostics: {string.Join(Environment.NewLine, compilation.GetDiagnostics().Select(d => d.ToString()))}");
   }
}

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Thinktecture.CodeAnalysis;

#nullable enable

namespace Thinktecture.Runtime.Tests.CodeAnalysis.TypedMemberStateFactoryProviderTests;

public class GetFactoryOrNull : CompilationTestBase
{
   [Fact]
   public void Should_return_null_when_compilation_has_error_type_for_System_Object()
   {
      // Create a compilation with no references, so System.Object will be an error type
      var source = "public class C { }";
      var syntaxTree = CSharpSyntaxTree.ParseText(source);

      var compilation = CSharpCompilation.Create(
         "TestAssembly",
         [syntaxTree],
         references: [], // No references means System.Object will be TypeKind.Error
         new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

      var objSymbol = compilation.GetSpecialType(SpecialType.System_Object);
      objSymbol.TypeKind.Should().Be(TypeKind.Error, "because compilation has no references");

      var factory = TypedMemberStateFactoryProvider.GetFactoryOrNull(compilation);

      factory.Should().BeNull("because System.Object is an error type");
   }

   [Fact]
   public void Should_return_factory_for_valid_compilation()
   {
      var source = """
         namespace N
         {
            public class C
            {
               public int Value { get; }
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var objSymbol = compilation.GetSpecialType(SpecialType.System_Object);
      objSymbol.TypeKind.Should().NotBe(TypeKind.Error, "because compilation has proper references");

      var factory = TypedMemberStateFactoryProvider.GetFactoryOrNull(compilation);

      factory.Should().NotBeNull("because compilation is valid");
   }

   [Fact]
   public void Should_return_same_factory_instance_for_same_compilation()
   {
      var source = """
         namespace N
         {
            public class C { }
         }
         """;

      var compilation = CreateCompilation(source);

      var factory1 = TypedMemberStateFactoryProvider.GetFactoryOrNull(compilation);
      var factory2 = TypedMemberStateFactoryProvider.GetFactoryOrNull(compilation);

      factory1.Should().NotBeNull();
      factory2.Should().NotBeNull();
      factory1.Should().BeSameAs(factory2, "because factory should be cached for the same .NET version");
   }

   [Fact]
   public void Should_return_same_factory_instance_for_different_compilations_with_same_dotnet_version()
   {
      var source1 = """
         namespace N1
         {
            public class C1 { }
         }
         """;

      var source2 = """
         namespace N2
         {
            public class C2 { }
         }
         """;

      var compilation1 = CreateCompilation(source1, assemblyName: "Assembly1");
      var compilation2 = CreateCompilation(source2, assemblyName: "Assembly2");

      var obj1 = compilation1.GetSpecialType(SpecialType.System_Object);
      var obj2 = compilation2.GetSpecialType(SpecialType.System_Object);

      var version1 = obj1.ContainingAssembly?.Identity.Version.Major ?? 0;
      var version2 = obj2.ContainingAssembly?.Identity.Version.Major ?? 0;

      version1.Should().Be(version2, "because both compilations use the same runtime");

      var factory1 = TypedMemberStateFactoryProvider.GetFactoryOrNull(compilation1);
      var factory2 = TypedMemberStateFactoryProvider.GetFactoryOrNull(compilation2);

      factory1.Should().NotBeNull();
      factory2.Should().NotBeNull();
      factory1.Should().BeSameAs(factory2, "because both compilations have the same .NET version");
   }

   [Fact]
   public void Should_handle_compilation_without_version_information()
   {
      var source = "public class C { }";
      var syntaxTree = CSharpSyntaxTree.ParseText(source);

      // Create a minimal reference that has System.Object but potentially no version info
      var coreAssembly = typeof(object).Assembly;
      var coreRef = MetadataReference.CreateFromFile(coreAssembly.Location);

      var compilation = CSharpCompilation.Create(
         "TestAssembly",
         [syntaxTree],
         references: [coreRef],
         new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

      var objSymbol = compilation.GetSpecialType(SpecialType.System_Object);
      objSymbol.TypeKind.Should().NotBe(TypeKind.Error);

      var factory = TypedMemberStateFactoryProvider.GetFactoryOrNull(compilation);

      factory.Should().NotBeNull("because compilation has valid System.Object");
   }

   [Fact]
   public async Task Should_return_same_factory_instance_when_called_concurrently()
   {
      var source = """
         namespace N
         {
            public class C { }
         }
         """;

      var compilation = CreateCompilation(source);

      // Call GetFactoryOrNull concurrently from multiple threads
      var tasks = Enumerable.Range(0, 10)
                            .Select(_ => Task.Run(() => TypedMemberStateFactoryProvider.GetFactoryOrNull(compilation)))
                            .ToList();

      var factories = await Task.WhenAll(tasks);

      factories.Should().AllSatisfy(f => f.Should().NotBeNull());

      var firstFactory = factories[0]!;
      factories.Should().AllSatisfy(f =>
                                       f.Should().BeSameAs(firstFactory, "because all concurrent calls should return the same cached instance"));
   }

   [Fact]
   public async Task Should_return_same_factory_for_different_compilations_with_same_version_when_called_concurrently()
   {
      // Create multiple compilations concurrently
      var compilations = Enumerable.Range(0, 10)
                                   .Select(i => CreateCompilation($"namespace N{i} {{ public class C{i} {{ }} }}", assemblyName: $"Assembly{i}"))
                                   .ToList();

      // Call GetFactoryOrNull concurrently for all compilations
      var tasks = compilations
                  .Select(c => Task.Run(() => TypedMemberStateFactoryProvider.GetFactoryOrNull(c)))
                  .ToList();

      var factories = await Task.WhenAll(tasks);

      factories.Should().AllSatisfy(f => f.Should().NotBeNull());

      var firstFactory = factories[0]!;
      factories.Should().AllSatisfy(f =>
                                       f.Should().BeSameAs(firstFactory, "because all compilations have the same .NET version"));
   }

   [Fact]
   public void Should_handle_empty_source_compilation()
   {
      var source = ""; // Empty source
      var compilation = CreateCompilation(source);

      var objSymbol = compilation.GetSpecialType(SpecialType.System_Object);
      objSymbol.TypeKind.Should().NotBe(TypeKind.Error, "because references are still present");

      var factory = TypedMemberStateFactoryProvider.GetFactoryOrNull(compilation);

      factory.Should().NotBeNull("because compilation still has valid System.Object");
   }

   [Fact]
   public void Should_handle_compilation_with_syntax_errors()
   {
      var source = """
         namespace N
         {
            public class C
            {
               invalid code
            }
         }
         """;

      var compilation = CreateCompilation(source, expectedCompilerErrors:
      [
         "; expected",
         "The type or namespace name 'invalid' could not be found (are you missing a using directive or an assembly reference?)"
      ]);

      var objSymbol = compilation.GetSpecialType(SpecialType.System_Object);
      objSymbol.TypeKind.Should().NotBe(TypeKind.Error, "because System.Object is still valid despite syntax errors");

      var factory = TypedMemberStateFactoryProvider.GetFactoryOrNull(compilation);

      factory.Should().NotBeNull("because System.Object is valid");
   }

   [Fact]
   public void Multiple_calls_should_use_cached_factory_not_recreate()
   {
      var source = """
         namespace N
         {
            public class C { }
         }
         """;

      var compilation = CreateCompilation(source);

      var factories = new List<TypedMemberStateFactory?>();

      // Call multiple times
      for (int i = 0; i < 100; i++)
      {
         factories.Add(TypedMemberStateFactoryProvider.GetFactoryOrNull(compilation));
      }

      factories.Should().AllSatisfy(f => f.Should().NotBeNull());

      var firstFactory = factories[0]!;
      factories.Should().AllSatisfy(f =>
                                       f.Should().BeSameAs(firstFactory, "because factory should be retrieved from cache, not recreated"));
   }

   [Fact]
   public void Should_handle_compilation_with_different_assembly_name()
   {
      var source = """
         namespace N
         {
            public class C { }
         }
         """;

      var compilation1 = CreateCompilation(source, assemblyName: "MyAssembly1");
      var compilation2 = CreateCompilation(source, assemblyName: "MyAssembly2");

      var factory1 = TypedMemberStateFactoryProvider.GetFactoryOrNull(compilation1);
      var factory2 = TypedMemberStateFactoryProvider.GetFactoryOrNull(compilation2);

      factory1.Should().NotBeNull();
      factory2.Should().NotBeNull();
      factory1.Should().BeSameAs(factory2, "because assembly name doesn't affect .NET version");
   }

   [Fact]
   public void Should_handle_compilation_with_different_compilation_options()
   {
      var source = """
         namespace N
         {
            public class C { }
         }
         """;

      var compilation1 = CreateCompilation(source, allowUnsafe: false);
      var compilation2 = CreateCompilation(source, allowUnsafe: true);

      var factory1 = TypedMemberStateFactoryProvider.GetFactoryOrNull(compilation1);
      var factory2 = TypedMemberStateFactoryProvider.GetFactoryOrNull(compilation2);

      factory1.Should().NotBeNull();
      factory2.Should().NotBeNull();
      factory1.Should().BeSameAs(factory2, "because compilation options don't affect .NET version");
   }

   [Fact]
   public void Should_handle_compilation_with_different_nullable_context()
   {
      var source = """
         namespace N
         {
            public class C { }
         }
         """;

      var compilation1 = CreateCompilation(source, nullableContextOptions: NullableContextOptions.Enable);
      var compilation2 = CreateCompilation(source, nullableContextOptions: NullableContextOptions.Disable);

      var factory1 = TypedMemberStateFactoryProvider.GetFactoryOrNull(compilation1);
      var factory2 = TypedMemberStateFactoryProvider.GetFactoryOrNull(compilation2);

      factory1.Should().NotBeNull();
      factory2.Should().NotBeNull();
      factory1.Should().BeSameAs(factory2, "because nullable context doesn't affect .NET version");
   }
}

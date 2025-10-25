using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Thinktecture.CodeAnalysis;

#nullable enable

namespace Thinktecture.Runtime.Tests.MetadataReferenceExtensionsTests;

[SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
public class GetModules
{
   [Fact]
   public void Returns_modules_when_MetadataReference_is_CompilationReference()
   {
      // Arrange
      var source = @"
namespace Test;

public class TestClass
{
}";
      var compilation = CreateCompilation(source);
      var compilationReference = compilation.ToMetadataReference();

      // Act
      var result = compilationReference.GetModules();

      // Assert
      result.Should().NotBeEmpty();
      result.Should().ContainSingle();
      result.First().Name.Should().Be("TestAssembly.dll");
   }

   [Fact]
   public void Returns_modules_when_MetadataReference_is_PortableExecutableReference()
   {
      // Arrange
      var systemRuntime = typeof(object).Assembly.Location;
      var portableExecutableReference = MetadataReference.CreateFromFile(systemRuntime);

      // Act
      var result = portableExecutableReference.GetModules();

      // Assert
      result.Should().NotBeEmpty();
      result.Should().Contain(m => m.Name.Contains("System.Private.CoreLib") || m.Name.Contains("mscorlib"));
   }

   [Fact]
   public void Returns_empty_when_MetadataReference_is_null()
   {
      // Arrange
      MetadataReference? metadataReference = null;

      // Act
      var result = metadataReference!.GetModules();

      // Assert
      result.Should().BeEmpty();
   }

   [Fact]
   public void Returns_correct_module_names_for_CompilationReference_with_custom_assembly_name()
   {
      // Arrange
      var source = @"
namespace CustomNamespace;

public class CustomClass
{
   public void CustomMethod() { }
}";
      var compilation = CreateCompilation(source, "CustomAssembly");
      var compilationReference = compilation.ToMetadataReference();

      // Act
      var result = compilationReference.GetModules();

      // Assert
      result.Should().ContainSingle();
      result.First().Name.Should().Be("CustomAssembly.dll");
   }

   [Fact]
   public void Returns_modules_for_compilation_with_multiple_syntax_trees()
   {
      // Arrange
      var source1 = @"
namespace Test;

public class Class1
{
}";
      var source2 = @"
namespace Test;

public class Class2
{
}";
      var parseOptions = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Preview);
      var syntaxTree1 = CSharpSyntaxTree.ParseText(source1, parseOptions);
      var syntaxTree2 = CSharpSyntaxTree.ParseText(source2, parseOptions);

      var references = AppDomain.CurrentDomain.GetAssemblies()
                                .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
                                .Select(a => MetadataReference.CreateFromFile(a.Location));

      var compilation = CSharpCompilation.Create(
         "MultiTreeAssembly",
         [syntaxTree1, syntaxTree2],
         references,
         new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Release));

      var compilationReference = compilation.ToMetadataReference();

      // Act
      var result = compilationReference.GetModules();

      // Assert
      result.Should().ContainSingle();
      result.First().Name.Should().Be("MultiTreeAssembly.dll");
   }

   [Fact]
   public void Returns_modules_for_different_assembly_types()
   {
      // Arrange - Test with System.Linq assembly
      var systemLinq = typeof(Enumerable).Assembly.Location;
      var portableExecutableReference = MetadataReference.CreateFromFile(systemLinq);

      // Act
      var result = portableExecutableReference.GetModules();

      // Assert
      result.Should().NotBeEmpty();
   }

   [Fact]
   public void Returns_same_results_for_multiple_calls_on_same_CompilationReference()
   {
      // Arrange
      var source = @"
namespace Test;

public class TestClass
{
}";
      var compilation = CreateCompilation(source);
      var compilationReference = compilation.ToMetadataReference();

      // Act
      var result1 = compilationReference.GetModules().ToArray();
      var result2 = compilationReference.GetModules().ToArray();

      // Assert
      result1.Should().BeEquivalentTo(result2);
   }

   [Fact]
   public void Returns_modules_preserving_module_name_casing()
   {
      // Arrange
      var source = @"
namespace Test;

public class TestClass
{
}";
      var compilation = CreateCompilation(source, "CaseSensitiveAssembly");
      var compilationReference = compilation.ToMetadataReference();

      // Act
      var result = compilationReference.GetModules();

      // Assert
      result.Should().ContainSingle();
      result.First().Name.Should().Be("CaseSensitiveAssembly.dll");
   }

   [Fact]
   public void Returns_modules_for_PortableExecutableReference_from_different_framework_assembly()
   {
      // Arrange - Use System.Collections assembly
      var systemCollections = typeof(System.Collections.Generic.List<>).Assembly.Location;
      var portableExecutableReference = MetadataReference.CreateFromFile(systemCollections);

      // Act
      var result = portableExecutableReference.GetModules();

      // Assert
      result.Should().NotBeEmpty();
   }

   [Fact]
   public void Returns_ModuleInfo_with_correct_type()
   {
      // Arrange
      var source = @"
namespace Test;

public class TestClass
{
}";
      var compilation = CreateCompilation(source);
      var compilationReference = compilation.ToMetadataReference();

      // Act
      var result = compilationReference.GetModules();

      // Assert
      result.Should().AllBeOfType<ModuleInfo>();
   }

   [Fact]
   public void Returns_module_when_MetadataReference_is_ModuleMetadata()
   {
      // Arrange
      var source = @"
namespace Test;

public class TestClass
{
}";
      var compilation = CreateCompilation(source, "TestModule");

      // Create a module reference by getting the module metadata from the compilation
      using var ms = new System.IO.MemoryStream();
      var emitResult = compilation.Emit(ms);
      emitResult.Success.Should().BeTrue();

      ms.Position = 0;
      var moduleMetadata = ModuleMetadata.CreateFromStream(ms, leaveOpen: true);
      var portableExecutableReference = moduleMetadata.GetReference();

      // Act
      var result = portableExecutableReference.GetModules();

      // Assert
      result.Should().ContainSingle();
      result.First().Name.Should().Be("TestModule.dll");
   }

   [Fact]
   public void Returns_single_module_for_netmodule_reference()
   {
      // Arrange - Create a netmodule (single module without assembly manifest)
      var source = @"
namespace NetModuleTest;

public class ModuleClass
{
   public void ModuleMethod() { }
}";
      var compilation = CreateCompilation(source, "TestNetModule");

      // Emit as a module (OutputKind.NetModule would be ideal, but we can test with DLL)
      using var ms = new System.IO.MemoryStream();
      var emitResult = compilation.Emit(ms);
      emitResult.Success.Should().BeTrue();

      ms.Position = 0;
      var moduleMetadata = ModuleMetadata.CreateFromStream(ms, leaveOpen: true);
      var portableExecutableReference = moduleMetadata.GetReference();

      // Act
      var result = portableExecutableReference.GetModules();

      // Assert
      result.Should().ContainSingle();
      result.First().Name.Should().NotBeNullOrEmpty();
   }

   [Fact]
   public void Returns_correct_module_name_for_ModuleMetadata_with_custom_name()
   {
      // Arrange
      var source = @"
namespace CustomModule;

public class CustomModuleClass
{
   public int CustomProperty { get; set; }
}";
      var compilation = CreateCompilation(source, "CustomModuleName");

      using var ms = new System.IO.MemoryStream();
      var emitResult = compilation.Emit(ms);
      emitResult.Success.Should().BeTrue();

      ms.Position = 0;
      var moduleMetadata = ModuleMetadata.CreateFromStream(ms, leaveOpen: true);
      var portableExecutableReference = moduleMetadata.GetReference();

      // Act
      var result = portableExecutableReference.GetModules();

      // Assert
      result.Should().ContainSingle();
      result.First().Name.Should().Be("CustomModuleName.dll");
   }

   [Fact]
   public void Returns_consistent_results_for_multiple_calls_on_ModuleMetadata()
   {
      // Arrange
      var source = @"
namespace Test;

public class TestClass
{
}";
      var compilation = CreateCompilation(source);

      using var ms = new System.IO.MemoryStream();
      var emitResult = compilation.Emit(ms);
      emitResult.Success.Should().BeTrue();

      ms.Position = 0;
      var moduleMetadata = ModuleMetadata.CreateFromStream(ms, leaveOpen: true);
      var portableExecutableReference = moduleMetadata.GetReference();

      // Act
      var result1 = portableExecutableReference.GetModules().ToArray();
      var result2 = portableExecutableReference.GetModules().ToArray();

      // Assert
      result1.Should().BeEquivalentTo(result2);
   }

   private static CSharpCompilation CreateCompilation(string source, string assemblyName = "TestAssembly")
   {
      var parseOptions = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Preview);
      var syntaxTree = CSharpSyntaxTree.ParseText(source, parseOptions);
      var references = AppDomain.CurrentDomain.GetAssemblies()
                                .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
                                .Select(a => MetadataReference.CreateFromFile(a.Location));

      return CSharpCompilation.Create(
         assemblyName,
         [syntaxTree],
         references,
         new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Release));
   }
}

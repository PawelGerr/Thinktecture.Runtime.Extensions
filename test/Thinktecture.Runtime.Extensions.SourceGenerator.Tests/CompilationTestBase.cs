using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#nullable enable

namespace Thinktecture.Runtime.Tests;

public abstract class CompilationTestBase
{
   protected static TypeDeclarationSyntax GetTypeDeclaration(string source, string typeName)
   {
      var syntaxTree = GetSyntaxTree(source);
      var root = syntaxTree.GetRoot();

      var typeDeclaration = root.DescendantNodes()
                                .OfType<TypeDeclarationSyntax>()
                                .FirstOrDefault(t => t.Identifier.Text == typeName);

      return typeDeclaration ?? throw new InvalidOperationException($"Type declaration '{typeName}' not found in source.");
   }

   protected static T GetMemberDeclaration<T>(string source, string memberName)
      where T : MemberDeclarationSyntax
   {
      var syntaxTree = GetSyntaxTree(source);
      var root = syntaxTree.GetRoot();

      var memberDeclaration = root.DescendantNodes()
                                  .OfType<T>()
                                  .FirstOrDefault(m =>
                                  {
                                     return m switch
                                     {
                                        MethodDeclarationSyntax method => method.Identifier.Text == memberName,
                                        _ => false
                                     };
                                  });

      return memberDeclaration ?? throw new InvalidOperationException($"Member declaration '{memberName}' not found in source.");
   }

   protected CSharpCompilation CreateCompilation(
      string source,
      string[] additionalReferences,
      string assemblyName = "TestAssembly",
      bool allowUnsafe = false,
      bool ignoreCompilationErrors = false,
      NullableContextOptions nullableContextOptions = default)
   {
      var additionalRefs = additionalReferences.Select(p => (MetadataReference)MetadataReference.CreateFromFile(p)).ToArray();

      return CreateCompilation(source, assemblyName, allowUnsafe, ignoreCompilationErrors, nullableContextOptions, additionalRefs);
   }

   protected CSharpCompilation CreateCompilation(
      string source,
      string assemblyName = "TestAssembly",
      bool allowUnsafe = false,
      bool ignoreCompilationErrors = false,
      NullableContextOptions nullableContextOptions = default,
      params MetadataReference[] additionalReferences)
   {
      var compilationOptions = new CSharpCompilationOptions(
         OutputKind.DynamicallyLinkedLibrary,
         optimizationLevel: OptimizationLevel.Release,
         nullableContextOptions: nullableContextOptions);

      if (allowUnsafe)
         compilationOptions = compilationOptions.WithAllowUnsafe(true);

      return CreateCompilation(source, compilationOptions, assemblyName, ignoreCompilationErrors, additionalReferences);
   }

   private static CSharpCompilation CreateCompilation(
      string source,
      CSharpCompilationOptions compilationOptions,
      string assemblyName = "TestAssembly",
      bool ignoreCompilationErrors = false,
      params MetadataReference[] additionalReferences)
   {
      var syntaxTree = GetSyntaxTree(source);
      var references = AppDomain.CurrentDomain.GetAssemblies()
                                .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
                                .Select(a => MetadataReference.CreateFromFile(a.Location))
                                .Concat(additionalReferences)
                                .ToList();

      var compilation = CSharpCompilation.Create(
         assemblyName,
         [syntaxTree],
         references,
         compilationOptions);

      if (!ignoreCompilationErrors)
         compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error).Should().BeEmpty();

      return compilation;
   }

   private static SyntaxTree GetSyntaxTree(string source)
   {
      var parseOptions = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Preview);
      var syntaxTree = CSharpSyntaxTree.ParseText(source, parseOptions);
      return syntaxTree;
   }

   protected static INamedTypeSymbol GetTypeSymbol(CSharpCompilation compilation, string metadataName)
   {
      var type = compilation.GetTypeByMetadataName(metadataName);
      return type ?? throw new InvalidOperationException($"Type '{metadataName}' not found. Diagnostics: {string.Join(Environment.NewLine, compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error).Select(d => d.ToString()))}");
   }
}

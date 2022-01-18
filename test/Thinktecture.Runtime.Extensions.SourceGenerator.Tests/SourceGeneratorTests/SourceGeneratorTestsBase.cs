using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit.Abstractions;

namespace Thinktecture.Runtime.Tests.SourceGeneratorTests;

public abstract class SourceGeneratorTestsBase
{
   private readonly ITestOutputHelper _output;

   protected SourceGeneratorTestsBase(ITestOutputHelper output)
   {
      _output = output ?? throw new ArgumentNullException(nameof(output));
   }

   protected string GetGeneratedOutput<T>(string source, params Assembly[] furtherAssemblies)
      where T : IIncrementalGenerator, new()
   {
      var syntaxTree = CSharpSyntaxTree.ParseText(source);
      var assemblies = new HashSet<Assembly>(AppDomain.CurrentDomain.GetAssemblies())
                       {
                          typeof(T).Assembly
                       };

      foreach (var furtherAssembly in furtherAssemblies)
      {
         assemblies.Add(furtherAssembly);
      }

      var references = assemblies.Where(assembly => !assembly.IsDynamic)
                                 .Select(assembly => MetadataReference.CreateFromFile(assembly.Location))
                                 .Cast<MetadataReference>();

      var compilation = CSharpCompilation.Create("SourceGeneratorTests", new[] { syntaxTree }, references, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

      // compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error).Should().BeEmpty();

      var generator = new T();
      CSharpGeneratorDriver.Create(generator).RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var generateDiagnostics);

      var errors = generateDiagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).ToList();
      errors.Should().BeEmpty();

      var output = outputCompilation.SyntaxTrees.Skip(1).LastOrDefault()?.ToString();

      _output.WriteLine(output ?? "No output provided.");

      return output;
   }

   protected static void AssertOutput(string output, string expectedOutput)
   {
      if (Environment.NewLine == "\n")
      {
         output = output?.Replace("\r\n", Environment.NewLine);
         expectedOutput = expectedOutput?.Replace("\r\n", Environment.NewLine);
      }

      output.Should().Be(expectedOutput);
   }
}

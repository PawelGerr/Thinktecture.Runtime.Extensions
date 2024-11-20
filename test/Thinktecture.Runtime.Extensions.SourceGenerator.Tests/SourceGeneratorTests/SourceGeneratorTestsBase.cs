using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using VerifyTests;
using VerifyXunit;
using Xunit.Abstractions;

namespace Thinktecture.Runtime.Tests.SourceGeneratorTests;

public abstract class SourceGeneratorTestsBase
{
   private const string _GENERATION_ERROR = "CS8785";

   protected static readonly VerifySettings Settings;

   static SourceGeneratorTestsBase()
   {
      Settings = new VerifySettings();
      Settings.UseDirectory("Snapshots");
   }

   private readonly ITestOutputHelper _output;

   protected SourceGeneratorTestsBase(ITestOutputHelper output)
   {
      _output = output ?? throw new ArgumentNullException(nameof(output));
   }

   protected async Task VerifyAsync(
      IReadOnlyDictionary<string, string> outputs,
      params string[] fileNames)
   {
      outputs.Should().HaveCount(fileNames.Length);

      var verifyTasks = fileNames.Select((fileName, i) =>
                                         {
                                            string content;

                                            try
                                            {
                                               content = outputs.Single(kvp => kvp.Key.Contains(fileName)).Value;
                                            }
                                            catch (Exception ex)
                                            {
                                               throw new Exception($"Output file '{fileName}' not found. Available files: {String.Join(", ", outputs.Keys)}", ex);
                                            }

                                            var verify = Verifier.Verify(content, "cs", Settings);

                                            if (fileNames.Length > 1)
                                               verify = verify.UseTextForParameters(i.ToString());

                                            return verify.ToTask();
                                         })
                                 .ToList();

      await Task.WhenAll(verifyTasks);
   }

   protected async Task VerifyAsync(
      string output)
   {
      await Verifier.Verify(output, "cs", Settings);
   }

   protected string GetGeneratedOutput<T>(
      string source,
      params Assembly[] furtherAssemblies)
      where T : IIncrementalGenerator, new()
   {
      return GetGeneratedOutput<T>(source, null, furtherAssemblies);
   }

   protected string GetGeneratedOutput<T>(
      string source,
      string generatedFileNameFragment,
      params Assembly[] furtherAssemblies)
      where T : IIncrementalGenerator, new()
   {
      var outputsByFilePath = GetGeneratedOutputs<T>(source, furtherAssemblies);

      var output = outputsByFilePath.SingleOrDefault(t => generatedFileNameFragment is null || t.Key.Contains(generatedFileNameFragment)).Value;

      _output.WriteLine(output ?? "No output provided.");

      return output;
   }

   public static Dictionary<string, string> GetGeneratedOutputs<T>(string source, params Assembly[] furtherAssemblies)
      where T : IIncrementalGenerator, new()
   {
      var syntaxTree = CSharpSyntaxTree.ParseText(source);
      var assemblies = new HashSet<Assembly>(AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName?.Contains("Thinktecture") != true))
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

      var compilation = CSharpCompilation.Create("SourceGeneratorTests",
                                                 new[] { syntaxTree },
                                                 references,
                                                 new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Release));

      var generator = new T();
      CSharpGeneratorDriver.Create(generator).RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var generateDiagnostics);

      var errors = generateDiagnostics.Where(d => d.Severity == DiagnosticSeverity.Error || d.Id == _GENERATION_ERROR).ToList();
      errors.Should().BeEmpty();

      return outputCompilation.SyntaxTrees
                              .Skip(1)
                              .ToDictionary(t => t.FilePath, t => t.ToString());
   }

   protected static void AssertOutput(string output, string expectedOutput)
   {
      output = output?.Replace("\r\n", "\n");
      expectedOutput = expectedOutput?.Replace("\r\n", "\n");

      output.Should().Be(expectedOutput);
   }
}

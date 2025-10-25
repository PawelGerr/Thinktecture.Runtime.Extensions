using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using VerifyXunit;
using Xunit.Abstractions;

namespace Thinktecture.Runtime.Tests.SourceGeneratorTests;

public abstract class SourceGeneratorTestsBase
{
   private const string _GENERATION_ERROR = "CS8785";

   private readonly ITestOutputHelper _output;
   private readonly int _maxOutputSize;

   protected SourceGeneratorTestsBase(
      ITestOutputHelper output,
      int maxOutputSize)
   {
      _output = output ?? throw new ArgumentNullException(nameof(output));
      _maxOutputSize = maxOutputSize;
   }

   protected Task VerifyAsync(
      IReadOnlyDictionary<string, string> outputs,
      params string[] fileNames)
   {
      return VerifyAsync(null, outputs, fileNames);
   }

   protected async Task VerifyAsync(
      string parameterText,
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

                                    if (content.Length > _maxOutputSize)
                                       throw new Exception($"Output file '{fileName}' is too big. Actual size: {content.Length}. Max size: {_maxOutputSize}.");

                                    var verify = Verifier.Verify(content);
                                    var paramText = parameterText;

                                    if (fileNames.Length > 1)
                                       paramText += i.ToString();

                                    if (!string.IsNullOrWhiteSpace(paramText))
                                       verify = verify.UseTextForParameters(paramText);

                                    return verify.ToTask();
                                 })
                                 .ToList();

      await Task.WhenAll(verifyTasks);
   }

   protected async Task VerifyAsync(
      string parameterText,
      string output)
   {
      if (output.Length > _maxOutputSize)
         throw new Exception($"Output file is too big. Actual size: {output.Length}. Max size: {_maxOutputSize}.");

      await Verifier.Verify(output)
                    .UseTextForParameters(parameterText);
   }

   protected async Task VerifyAsync(
      string output)
   {
      if (output?.Length > _maxOutputSize)
         throw new Exception($"Output file is too big. Actual size: {output.Length}. Max size: {_maxOutputSize}.");

      await Verifier.Verify(output);
   }

   protected string GetGeneratedOutput<T>(
      string source,
      params Assembly[] furtherAssemblies)
      where T : IIncrementalGenerator, new()
   {
      return GetGeneratedOutput<T>(source, furtherAssemblies, []);
   }

   protected string GetGeneratedOutput<T>(
      string source,
      Assembly[] furtherAssemblies,
      string[] expectedCompilerErrors)
      where T : IIncrementalGenerator, new()
   {
      return GetGeneratedOutput<T>(source, null, furtherAssemblies, expectedCompilerErrors);
   }

   protected string GetGeneratedOutput<T>(
      string source,
      string generatedFileNameFragment,
      params Assembly[] furtherAssemblies)
      where T : IIncrementalGenerator, new()
   {
      return GetGeneratedOutput<T>(source, generatedFileNameFragment, furtherAssemblies, []);
   }

   protected string GetGeneratedOutput<T>(
      string source,
      string generatedFileNameFragment,
      Assembly[] furtherAssemblies,
      string[] expectedCompilerErrors)
      where T : IIncrementalGenerator, new()
   {
      var outputsByFilePath = GetGeneratedOutputs<T>(source, furtherAssemblies, expectedCompilerErrors);

      var output = outputsByFilePath.SingleOrDefault(t => generatedFileNameFragment is null || t.Key.Contains(generatedFileNameFragment)).Value;

      _output.WriteLine(output ?? "No output provided.");

      return output;
   }

   protected static Dictionary<string, string> GetGeneratedOutputs<T>(
      string source,
      params Assembly[] furtherAssemblies)
      where T : IIncrementalGenerator, new()
   {
      return GetGeneratedOutputs<T>(source, furtherAssemblies, []);
   }

   protected static Dictionary<string, string> GetGeneratedOutputs<T>(
      string source,
      Assembly[] furtherAssemblies,
      string[] expectedCompilerErrors)
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
                                                 [syntaxTree],
                                                 references,
                                                 new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Release));

      var generator = new T();
      CSharpGeneratorDriver.Create(generator).RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var generateDiagnostics);

      var errors = generateDiagnostics.Where(d => d.Severity == DiagnosticSeverity.Error || d.Id == _GENERATION_ERROR).ToList();
      errors.Where(e => !expectedCompilerErrors.Contains(e.GetMessage())).Should().BeEmpty();

      return outputCompilation.SyntaxTrees
                              .Skip(1)
                              .ToDictionary(t => t.FilePath, t => t.ToString());
   }
}

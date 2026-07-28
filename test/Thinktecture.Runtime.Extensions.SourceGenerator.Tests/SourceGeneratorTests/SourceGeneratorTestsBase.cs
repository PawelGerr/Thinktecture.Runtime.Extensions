using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Thinktecture.CodeAnalysis.AdHocUnions;
using Thinktecture.CodeAnalysis.Annotations;
using Thinktecture.CodeAnalysis.ObjectFactories;
using Thinktecture.CodeAnalysis.RegularUnions;
using Thinktecture.CodeAnalysis.SmartEnums;
using Thinktecture.CodeAnalysis.ValueObjects;
using VerifyXunit;

namespace Thinktecture.Runtime.Tests.SourceGeneratorTests;

public abstract class SourceGeneratorTestsBase
{
   private const string _GENERATION_ERROR = "CS8785";
   private const string _PARTIAL_METHOD_MUST_HAVE_IMPLEMENTATION = "CS8795";

   // The input compilation is incomplete by definition: the constructor of a Smart Enum is generated, so a
   // derived item calling ": base(key)" cannot bind before generation. Same category as CS8795.
   private const string _CONSTRUCTOR_NOT_FOUND = "CS1729";

   // The generated code branches on the target framework. Without these symbols only the pre-NET9 branch of
   // every "#if NET9_0_OR_GREATER" would ever be compiled, so half of the generated code stays unchecked.
   // The list is derived from the framework this test assembly was compiled for, so a new target framework
   // needs no change here.
   private static readonly CSharpParseOptions _parseOptions =
      CSharpParseOptions.Default.WithPreprocessorSymbols(
         Enumerable.Range(8, GetTargetFrameworkMajorVersion() - 7).Select(version => $"NET{version}_0_OR_GREATER"));

   private static int GetTargetFrameworkMajorVersion()
   {
      var frameworkName = typeof(SourceGeneratorTestsBase).Assembly
                                                          .GetCustomAttribute<TargetFrameworkAttribute>()
                                                          ?.FrameworkName
                          ?? throw new InvalidOperationException("The test assembly has no TargetFrameworkAttribute.");

      return new FrameworkName(frameworkName).Version.Major;
   }

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

      var verifyTasks = fileNames.Select(fileName =>
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
                                    var paramText = parameterText + (String.IsNullOrWhiteSpace(parameterText) || parameterText.EndsWith("_") ? "file=" : "_file=") + fileName.Replace("`", "");

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
      var output = GetGeneratedOutputs<T>(source, generatedFileNameFragment, furtherAssemblies, expectedCompilerErrors).SingleOrDefault().Value;

      _output.WriteLine(output ?? "No output provided.");

      return output;
   }

   protected static Dictionary<string, string> GetGeneratedOutputs<T>(
      string source,
      params Assembly[] furtherAssemblies)
      where T : IIncrementalGenerator, new()
   {
      return GetGeneratedOutputs<T>(source, null, furtherAssemblies);
   }

   protected static Dictionary<string, string> GetGeneratedOutputs<T>(
      string source,
      string generatedFileNameFragment,
      params Assembly[] furtherAssemblies)
      where T : IIncrementalGenerator, new()
   {
      return GetGeneratedOutputs<T>(source, generatedFileNameFragment, furtherAssemblies, []);
   }

   protected static Dictionary<string, string> GetGeneratedOutputs<T>(
      string source,
      Assembly[] furtherAssemblies,
      string[] expectedCompilerErrors)
      where T : IIncrementalGenerator, new()
   {
      return GetGeneratedOutputs<T>(source, null, furtherAssemblies, expectedCompilerErrors);
   }

   protected static Dictionary<string, string> GetGeneratedOutputs<T>(
      string source,
      string generatedFileNameFragment,
      Assembly[] furtherAssemblies,
      string[] expectedCompilerErrors)
      where T : IIncrementalGenerator, new()
   {
      var result = RunGenerator<T>(source, generatedFileNameFragment, furtherAssemblies, expectedCompilerErrors, assertNoUnexpectedGeneratorErrors: true);
      return result.Outputs;
   }

   protected record GeneratorResult(
      Dictionary<string, string> Outputs,
      ImmutableArray<Diagnostic> GeneratorDiagnostics);

   protected static GeneratorResult GetGeneratedOutputsWithDiagnostics<T>(
      string source,
      params Assembly[] furtherAssemblies)
      where T : IIncrementalGenerator, new()
   {
      return RunGenerator<T>(source, null, furtherAssemblies, [], assertNoUnexpectedGeneratorErrors: false);
   }

   /// <summary>
   /// Runs the generator twice on the same driver instance: first on <paramref name="initialSource"/>,
   /// then on <paramref name="editedSource"/> after replacing the syntax tree. Reusing the driver is what
   /// exercises the incremental cache: if a state object compares equal despite a relevant change, the
   /// driver keeps the stale output from the first run instead of regenerating it. Returns the generated
   /// outputs of the second (post-edit) run, filtered by <paramref name="generatedFileNameFragment"/>.
   /// </summary>
   protected static Dictionary<string, string> GetGeneratedOutputsAfterEdit<T>(
      string initialSource,
      string editedSource,
      string generatedFileNameFragment,
      params Assembly[] furtherAssemblies)
      where T : IIncrementalGenerator, new()
   {
      var initialTree = CSharpSyntaxTree.ParseText(initialSource);
      var editedTree = CSharpSyntaxTree.ParseText(editedSource);

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
                                                 [initialTree],
                                                 references,
                                                 new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Release));

      GeneratorDriver driver = CSharpGeneratorDriver.Create(new T());
      driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out _, out _);

      var editedCompilation = compilation.ReplaceSyntaxTree(initialTree, editedTree);
      driver.RunGeneratorsAndUpdateCompilation(editedCompilation, out var outputCompilation, out _);

      return outputCompilation.SyntaxTrees
                              .Skip(1)
                              .Where(t => generatedFileNameFragment is null || t.FilePath.Contains(generatedFileNameFragment))
                              .ToDictionary(t => t.FilePath, t => t.ToString());
   }

   /// <summary>
   /// The generated code must compile. Without this check a snapshot test passes even when the generator
   /// emits code that no user could ever build, which is how an incomplete Switch/Map rename once shipped.
   /// Errors already present in <paramref name="input"/> are skipped, because the input compilation is
   /// asserted separately and reports its own incompleteness.
   /// </summary>
   private static void AssertGeneratedCodeCompiles(
      Compilation input,
      Compilation output,
      string[] expectedCompilerErrors)
   {
      var inputErrors = input.GetDiagnostics()
                             .Where(d => d.Severity == DiagnosticSeverity.Error)
                             .Select(d => d.Id + "|" + d.GetMessage())
                             .ToHashSet();

      var errors = output.GetDiagnostics()
                         .Where(d => d.Severity == DiagnosticSeverity.Error)
                         .Where(d => !inputErrors.Contains(d.Id + "|" + d.GetMessage()))
                         .Where(d => !expectedCompilerErrors.Contains(d.GetMessage()))
                         .ToList();

      errors.Should().BeEmpty();
   }

   /// <summary>
   /// Every generator except the one under test. A real build runs all of them, and their outputs depend on
   /// each other: the <c>InstantHandleAttribute</c>, the <c>IParsable</c> members of a type with an
   /// <c>[ObjectFactory&lt;T&gt;]</c>, and the <c>Validate</c> member the Smart Enum / Value Object
   /// generator supplies. Leaving one out makes the compile gate report members that a real build has.
   /// </summary>
   private static IEnumerable<IIncrementalGenerator> CompanionGenerators<T>()
      where T : IIncrementalGenerator, new()
   {
      IIncrementalGenerator[] companions =
      [
         new AnnotationsSourceGenerator(),
         new ObjectFactorySourceGenerator(),
         new AdHocUnionSourceGenerator(),
         new RegularUnionSourceGenerator(),
         new SmartEnumSourceGenerator(),
         new ValueObjectSourceGenerator()
      ];

      return companions.Where(g => g.GetType() != typeof(T));
   }

   private static GeneratorResult RunGenerator<T>(
      string source,
      string generatedFileNameFragment,
      Assembly[] furtherAssemblies,
      string[] expectedCompilerErrors,
      bool assertNoUnexpectedGeneratorErrors)
      where T : IIncrementalGenerator, new()
   {
      var syntaxTree = CSharpSyntaxTree.ParseText(source, _parseOptions);
      var assemblies = new HashSet<Assembly>(AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName?.Contains("Thinktecture") != true))
                       {
                          typeof(T).Assembly,
                          typeof(System.ComponentModel.TypeConverterAttribute).Assembly,
                          typeof(System.ComponentModel.DataAnnotations.ValidationAttribute).Assembly,
                          typeof(System.Linq.Expressions.Expression).Assembly
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

      var errors = compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error && d.Id != _PARTIAL_METHOD_MUST_HAVE_IMPLEMENTATION && d.Id != _CONSTRUCTOR_NOT_FOUND).ToList();
      errors.Where(e => !expectedCompilerErrors.Contains(e.GetMessage())).Should().BeEmpty();

      var generator = new T();

      IIncrementalGenerator[] generators = [generator, .. CompanionGenerators<T>()];

      var driver = CSharpGeneratorDriver.Create(generators.Select(g => g.AsSourceGenerator()), parseOptions: _parseOptions)
                                        .RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var generateDiagnostics);

      if (assertNoUnexpectedGeneratorErrors)
      {
         errors = generateDiagnostics.Where(d => d.Severity == DiagnosticSeverity.Error || d.Id == _GENERATION_ERROR).ToList();
         errors.Where(e => !expectedCompilerErrors.Contains(e.GetMessage())).Should().BeEmpty();
      }

      AssertGeneratedCodeCompiles(compilation, outputCompilation, expectedCompilerErrors);

      var outputs = driver.GetRunResult().Results
                          .Where(r => r.Generator.GetGeneratorType() == typeof(T))
                          .SelectMany(r => r.GeneratedSources)
                          .Where(s => generatedFileNameFragment is null || s.SyntaxTree.FilePath.Contains(generatedFileNameFragment))
                          .ToDictionary(s => s.SyntaxTree.FilePath, s => s.SyntaxTree.ToString());

      return new GeneratorResult(outputs, generateDiagnostics);
   }
}

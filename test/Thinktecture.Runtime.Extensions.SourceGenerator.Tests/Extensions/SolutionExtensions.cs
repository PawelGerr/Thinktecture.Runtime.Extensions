using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Testing;

// ReSharper disable once CheckNamespace
namespace Thinktecture.Extensions;

public static class SolutionExtensions
{
   /// <summary>
   /// By default, the compiler reports diagnostics for nullable reference types at
   /// <see cref="DiagnosticSeverity.Warning"/>, and the analyzer test framework defaults to only validating
   /// diagnostics at <see cref="DiagnosticSeverity.Error"/>. This map contains all compiler diagnostic IDs
   /// related to nullability mapped to <see cref="ReportDiagnostic.Error"/>, which is then used to enable all
   /// of these warnings for default validation during analyzer and code fix tests.
   /// </summary>
   private static IReadOnlyDictionary<string, ReportDiagnostic> NullableWarnings { get; } = GetNullableWarningsFromCompiler();

   private static IReadOnlyDictionary<string, ReportDiagnostic> GetNullableWarningsFromCompiler()
   {
      string[] args = { "/warnaserror:nullable" };
      var commandLineArguments = CSharpCommandLineParser.Default.Parse(args, Environment.CurrentDirectory, Environment.CurrentDirectory);
      var nullableWarnings = commandLineArguments.CompilationOptions.SpecificDiagnosticOptions;

      // Workaround for https://github.com/dotnet/roslyn/issues/41610
      nullableWarnings = nullableWarnings
                         .SetItem("CS8632", ReportDiagnostic.Error)
                         .SetItem("CS8669", ReportDiagnostic.Error);

      return nullableWarnings;
   }

   public static void DisableNullableReferenceWarnings<TVerifier>(this AnalyzerTest<TVerifier> test)
      where TVerifier : IVerifier, new()
   {
      if (test is null)
         throw new ArgumentNullException(nameof(test));

      test.SolutionTransforms.Add(DisableNullableWarnings);
   }

   private static Solution DisableNullableWarnings(Solution solution, ProjectId projectId)
   {
      var compilationOptions = solution.GetProject(projectId).CompilationOptions;
      compilationOptions = compilationOptions.WithSpecificDiagnosticOptions(compilationOptions.SpecificDiagnosticOptions.SetItems(NullableWarnings));
      solution = solution.WithProjectCompilationOptions(projectId, compilationOptions);

      return solution;
   }
}
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Thinktecture.Extensions;

namespace Thinktecture.Verifiers
{
   public static class AnalyzerVerifier<TAnalyzer>
      where TAnalyzer : DiagnosticAnalyzer, new()
   {
      public static DiagnosticResult Diagnostic(string diagnosticId)
      {
         return CSharpAnalyzerVerifier<TAnalyzer, XUnitVerifier>.Diagnostic(diagnosticId);
      }

      public static Task VerifyAnalyzerAsync(
         string source,
         params DiagnosticResult[] expected)
      {
         return VerifyAnalyzerAsync(source, Array.Empty<Assembly>(), expected);
      }

      public static async Task VerifyAnalyzerAsync(
         string source,
         IEnumerable<Assembly> additionalReferences,
         params DiagnosticResult[] expected)
      {
         var test = new AnalyzerTest(source, additionalReferences, expected);
         await test.RunAsync(CancellationToken.None);
      }

      private class AnalyzerTest : CSharpAnalyzerTest<TAnalyzer, XUnitVerifier>
      {
         public AnalyzerTest(
            string source,
            IEnumerable<Assembly> additionalReferences,
            params DiagnosticResult[] expected)
         {
            TestCode = source;
            ExpectedDiagnostics.AddRange(expected);
            ReferenceAssemblies = ReferenceAssemblies.Net.Net50;

            foreach (var additionalReference in additionalReferences)
            {
               TestState.AdditionalReferences.Add(additionalReference);
            }

            this.DisableNullableReferenceWarnings();
         }
      }
   }
}

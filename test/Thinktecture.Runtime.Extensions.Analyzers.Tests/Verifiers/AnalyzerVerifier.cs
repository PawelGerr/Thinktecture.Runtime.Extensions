using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Thinktecture.Extensions;

namespace Thinktecture.Runtime.Tests.Verifiers;

public static class AnalyzerVerifier<TAnalyzer>
   where TAnalyzer : DiagnosticAnalyzer, new()
{
   public static DiagnosticResult Diagnostic(string diagnosticId)
   {
      return CSharpAnalyzerVerifier<TAnalyzer, DefaultVerifier>.Diagnostic(diagnosticId);
   }

   public static Task VerifyAnalyzerAsync(
      string source,
      params DiagnosticResult[] expected)
   {
      return VerifyAnalyzerAsync(source, [], expected);
   }

   public static async Task VerifyAnalyzerAsync(
      string source,
      IEnumerable<Assembly> additionalReferences,
      params DiagnosticResult[] expected)
   {
      var test = new AnalyzerTest(source, additionalReferences, expected);
      await test.RunAsync(CancellationToken.None);
   }

   private class AnalyzerTest : CSharpAnalyzerTest<TAnalyzer, DefaultVerifier>
   {
      public AnalyzerTest(
         string source,
         IEnumerable<Assembly> additionalReferences,
         params DiagnosticResult[] expected)
      {
         TestCode = source;
         ExpectedDiagnostics.AddRange(expected);
#if NET8_0
         ReferenceAssemblies = new ReferenceAssemblies("net8.0", new PackageIdentity("Microsoft.NETCore.App.Ref", "8.0.0"), Path.Combine("ref", "8.0.0"));
#elif NET9_0
         ReferenceAssemblies = new ReferenceAssemblies("net9.0", new PackageIdentity("Microsoft.NETCore.App.Ref", "9.0.0"), Path.Combine("ref", "9.0.0"));
#else
         ReferenceAssemblies = new ReferenceAssemblies("net10.0", new PackageIdentity("Microsoft.NETCore.App.Ref", "10.0.0"), Path.Combine("ref", "10.0.0"));
#endif

         foreach (var additionalReference in additionalReferences)
         {
            TestState.AdditionalReferences.Add(additionalReference);
         }

         this.DisableNullableReferenceWarnings();
      }
   }
}

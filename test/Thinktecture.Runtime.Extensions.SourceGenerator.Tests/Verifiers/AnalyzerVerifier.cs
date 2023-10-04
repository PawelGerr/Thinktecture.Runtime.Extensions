using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Thinktecture.Extensions;

namespace Thinktecture.Runtime.Tests.Verifiers;

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
#if NET7
         ReferenceAssemblies = new ReferenceAssemblies("net7.0", new PackageIdentity("Microsoft.NETCore.App.Ref","7.0.0"), Path.Combine("ref", "7.0.0"));
#else
         ReferenceAssemblies = new ReferenceAssemblies("net8.0", new PackageIdentity("Microsoft.NETCore.App.Ref","8.0.0-rc.1.23419.4"), Path.Combine("ref", "8.0.0-rc.1.23419.4"));
#endif
         foreach (var additionalReference in additionalReferences)
         {
            TestState.AdditionalReferences.Add(additionalReference);
         }

         this.DisableNullableReferenceWarnings();
      }
   }
}

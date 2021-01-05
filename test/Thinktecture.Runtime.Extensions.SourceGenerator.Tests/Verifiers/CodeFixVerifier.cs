﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Thinktecture.Extensions;

namespace Thinktecture.Verifiers
{
   public static class CodeFixVerifier<TAnalyzer, TCodeFix>
      where TAnalyzer : DiagnosticAnalyzer, new()
      where TCodeFix : CodeFixProvider, new()
   {
      public static DiagnosticResult Diagnostic(string diagnosticId)
      {
         return CSharpCodeFixVerifier<TAnalyzer, TCodeFix, XUnitVerifier>.Diagnostic(diagnosticId);
      }

      public static async Task VerifyCodeFixAsync(
         string source,
         string fixedSource,
         IEnumerable<Assembly> additionalReferences,
         params DiagnosticResult[] expected)
      {
         var test = new CodeFixTest(source, fixedSource, additionalReferences, expected);
         await test.RunAsync(CancellationToken.None);
      }

      private class CodeFixTest : CSharpCodeFixTest<TAnalyzer, TCodeFix, XUnitVerifier>
      {
         public CodeFixTest(
            string source,
            string fixedSource,
            IEnumerable<Assembly> additionalReferences,
            params DiagnosticResult[] expected)
         {
            TestCode = source;
            FixedCode = fixedSource;
            ExpectedDiagnostics.AddRange(expected);

            foreach (var additionalReference in additionalReferences)
            {
               TestState.AdditionalReferences.Add(additionalReference);
            }

            this.DisableNullableReferenceWarnings();
         }
      }
   }
}

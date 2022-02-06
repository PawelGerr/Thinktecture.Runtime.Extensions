﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Thinktecture.Extensions;

namespace Thinktecture.Runtime.Tests.Verifiers;

public static class CodeFixVerifier<TAnalyzer, TCodeFix>
   where TAnalyzer : DiagnosticAnalyzer, new()
   where TCodeFix : CodeFixProvider, new()
{
   public static DiagnosticResult Diagnostic(string diagnosticId)
   {
      return CSharpCodeFixVerifier<TAnalyzer, TCodeFix, XUnitVerifier>.Diagnostic(diagnosticId);
   }

   public static async Task VerifyAnalyzerAsync(
      string source,
      IEnumerable<Assembly> additionalReferences,
      params DiagnosticResult[] expected)
   {
      var test = new CodeFixTest(source, null, additionalReferences, expected);
      await test.RunAsync(CancellationToken.None);
   }

   public static async Task VerifyCodeFixAsync(
      string source,
      string fixedSource,
      IEnumerable<Assembly> additionalReferences,
      params DiagnosticResult[] expected)
   {
      if (Environment.NewLine == "\n")
      {
         source = source.Replace("\r\n", Environment.NewLine);
         fixedSource = fixedSource.Replace("\r\n", Environment.NewLine);
      }

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
#if NET6_0
         ReferenceAssemblies = new ReferenceAssemblies("net6.0", new PackageIdentity("Microsoft.NETCore.App.Ref","6.0.0"), Path.Combine("ref", "net6.0"));
#else
         ReferenceAssemblies = ReferenceAssemblies.Net.Net50;
#endif

         foreach (var additionalReference in additionalReferences)
         {
            TestState.AdditionalReferences.Add(additionalReference);
         }

         this.DisableNullableReferenceWarnings();
      }
   }
}

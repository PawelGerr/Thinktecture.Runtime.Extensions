using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Thinktecture.Extensions;

namespace Thinktecture.Runtime.Tests.Verifiers;

public static class CodeFixVerifier<TAnalyzer, TCodeFix>
   where TAnalyzer : DiagnosticAnalyzer, new()
   where TCodeFix : CodeFixProvider, new()
{
   public static DiagnosticResult Diagnostic(string diagnosticId)
   {
      return CSharpCodeFixVerifier<TAnalyzer, TCodeFix, DefaultVerifier>.Diagnostic(diagnosticId);
   }

   public static DiagnosticResult Diagnostic(string diagnosticId, string message)
   {
      return CSharpCodeFixVerifier<TAnalyzer, TCodeFix, DefaultVerifier>.Diagnostic(new DiagnosticDescriptor(diagnosticId, "", message, "", DiagnosticSeverity.Error, true));
   }

   public static DiagnosticResult Diagnostic(DiagnosticDescriptor diagnosticDescriptor)
   {
      return CSharpCodeFixVerifier<TAnalyzer, TCodeFix, DefaultVerifier>.Diagnostic(diagnosticDescriptor);
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

   private class CodeFixTest : CSharpCodeFixTest<TAnalyzer, TCodeFix, DefaultVerifier>
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
#if NET7
         ReferenceAssemblies = new ReferenceAssemblies("net7.0", new PackageIdentity("Microsoft.NETCore.App.Ref","7.0.0"), Path.Combine("ref", "7.0.0"));
#elif NET8_0
         ReferenceAssemblies = new ReferenceAssemblies("net8.0", new PackageIdentity("Microsoft.NETCore.App.Ref", "8.0.0"), Path.Combine("ref", "8.0.0"));
#else
         ReferenceAssemblies = new ReferenceAssemblies("net9.0", new PackageIdentity("Microsoft.NETCore.App.Ref", "9.0.0"), Path.Combine("ref", "9.0.0"));
#endif

         foreach (var additionalReference in additionalReferences)
         {
            TestState.AdditionalReferences.Add(additionalReference);
         }

         this.DisableNullableReferenceWarnings();
      }

      protected override ParseOptions CreateParseOptions()
      {
         var options = (CSharpParseOptions)base.CreateParseOptions();
         options = options.WithLanguageVersion(LanguageVersion.Preview);

         return options;
      }
   }
}

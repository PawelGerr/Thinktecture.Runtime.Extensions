using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Thinktecture.Extensions;

namespace Thinktecture.Runtime.Tests.Verifiers;

public static class CodeRefactoringVerifier<TRefactoring>
   where TRefactoring : CodeRefactoringProvider, new()
{
   public static async Task VerifyRefactoringAsync(
      string source,
      string fixedSource,
      IEnumerable<Assembly> additionalReferences,
      int? codeActionIndex = null)
   {
      var test = new RefactoringTest(source, fixedSource, additionalReferences, codeActionIndex);
      await test.RunAsync(CancellationToken.None);
   }

   public static async Task VerifyNoRefactoringAsync(
      string source,
      IEnumerable<Assembly> additionalReferences)
   {
      var test = new RefactoringTest(source, source, additionalReferences, null)
                 {
                    CodeActionValidationMode = CodeActionValidationMode.None
                 };

      await test.RunAsync(CancellationToken.None);
   }

   private class RefactoringTest : CSharpCodeRefactoringTest<TRefactoring, DefaultVerifier>
   {
      public RefactoringTest(
         string source,
         string fixedSource,
         IEnumerable<Assembly> additionalReferences,
         int? codeActionIndex)
      {
         TestCode = source.NormalizeLineEndings();
         FixedCode = fixedSource.NormalizeLineEndings();

         if (codeActionIndex.HasValue)
            CodeActionIndex = codeActionIndex.Value;

#if NET8_0
         ReferenceAssemblies = new ReferenceAssemblies("net8.0", new PackageIdentity("Microsoft.NETCore.App.Ref", "8.0.0"), Path.Combine("ref", "8.0.0"));
#elif NET9_0
         ReferenceAssemblies = new ReferenceAssemblies("net9.0", new PackageIdentity("Microsoft.NETCore.App.Ref", "9.0.0"), Path.Combine("ref", "9.0.0"));
#else
         ReferenceAssemblies = new ReferenceAssemblies("net10.0", new PackageIdentity("Microsoft.NETCore.App.Ref", "10.0.0"), Path.Combine("ref", "10.0.0"));
#endif

         CompilerDiagnostics = CompilerDiagnostics.None;

         foreach (var additionalReference in additionalReferences)
         {
            TestState.AdditionalReferences.Add(additionalReference);
         }

         this.DisableNullableReferenceWarnings();
      }

      protected override ParseOptions CreateParseOptions()
      {
         var options = (CSharpParseOptions)base.CreateParseOptions();
         options = options.WithLanguageVersion(LanguageVersion.CSharp14);

         return options;
      }
   }
}

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

// ReSharper disable once CheckNamespace
namespace Thinktecture
{
   public static class GeneratorExecutionContextExtensions
   {
      public static void ReportTypeCouldNotBeResolved(this GeneratorExecutionContext context, TypeSyntax typeSyntax, SyntaxToken identifier)
      {
         context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.MultipleIncompatibleEnumInterfaces, typeSyntax.GetLocation(), identifier));
      }
   }
}

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Thinktecture.CodeAnalysis;

namespace Thinktecture;

public static class SourceProductionContextExtensions
{
   public static void EmitFile(this SourceProductionContext context, string? typeNamespace, string typeName, string? generatedCode, string? fileNameSuffix)
   {
      if (String.IsNullOrWhiteSpace(generatedCode))
         return;

      var hintName = $"{(typeNamespace is null ? null : $"{typeNamespace}.")}{typeName}{fileNameSuffix}.g.cs";
      context.AddSource(hintName, generatedCode!);
   }

   public static void ReportError(this SourceProductionContext context, TypeDeclarationSyntax node, string message)
   {
      context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.ErrorDuringGeneration,
                                                 node.GetLocation(),
                                                 node.Identifier.Text, message));
   }
}

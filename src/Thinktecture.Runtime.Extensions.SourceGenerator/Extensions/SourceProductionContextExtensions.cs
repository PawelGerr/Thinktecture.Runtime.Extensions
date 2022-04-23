using Microsoft.CodeAnalysis;
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

   public static void ReportException(this SourceProductionContext context, Exception ex)
   {
      context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.ErrorDuringGeneration,
                                                 null,
                                                 new object?[] { null, ex.Message }));
   }
}

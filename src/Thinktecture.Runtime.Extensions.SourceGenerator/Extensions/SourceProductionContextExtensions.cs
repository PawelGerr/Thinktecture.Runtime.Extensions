using Microsoft.CodeAnalysis;
using Thinktecture.CodeAnalysis;

namespace Thinktecture;

public static class SourceProductionContextExtensions
{
   public static void ReportException(this SourceProductionContext context, Exception ex)
   {
      context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.ErrorDuringGeneration,
                                                 null,
                                                 new object?[] { null, ex.Message }));
   }
}

using Microsoft.CodeAnalysis;
using Thinktecture.CodeAnalysis;

namespace Thinktecture;

public static class SourceGeneratorStateExtensions
{
   public static Location? GetLocationOrNullSafe(this ISourceGeneratorState enumState, SourceProductionContext context)
   {
      try
      {
         // pick one location as the representative
         return enumState.GetFirstLocation(context.CancellationToken);
      }
      catch (Exception ex)
      {
         context.ReportException(ex);
         return null;
      }
   }
}

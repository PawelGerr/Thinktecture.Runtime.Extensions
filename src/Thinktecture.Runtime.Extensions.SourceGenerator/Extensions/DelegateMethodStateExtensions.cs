using Thinktecture.CodeAnalysis;

namespace Thinktecture;

public static class DelegateMethodStateExtensions
{
   public static bool NeedsCustomDelegate(this DelegateMethodState method)
   {
      return method.DelegateName != null || method.Parameters.Any(p => p.RefKind != RefKind.None);
   }
}

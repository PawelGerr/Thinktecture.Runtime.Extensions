using Thinktecture.CodeAnalysis;

namespace Thinktecture;

public static class DelegateMethodStateExtensions
{
   public static bool NeedsCustomDelegate(this DelegateMethodState method)
   {
      if (method.DelegateName != null)
         return true;

      for (var i = 0; i < method.Parameters.Length; i++)
      {
         if (method.Parameters[i].RefKind != RefKind.None)
            return true;
      }

      return false;
   }
}

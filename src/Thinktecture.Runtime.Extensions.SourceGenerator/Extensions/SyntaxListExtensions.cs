using Microsoft.CodeAnalysis;

namespace Thinktecture;

public static class SyntaxListExtensions
{
   public static T? FirstOrDefault<T>(this SyntaxList<T> list, Func<T, bool> predicate)
      where T : SyntaxNode
   {
      for (var i = 0; i < list.Count; i++)
      {
         var node = list[i];

         if (predicate(node))
            return node;
      }

      return default;
   }
}

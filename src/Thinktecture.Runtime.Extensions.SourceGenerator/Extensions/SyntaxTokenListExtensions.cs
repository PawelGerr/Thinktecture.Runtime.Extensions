namespace Thinktecture;

public static class SyntaxTokenListExtensions
{
   public static SyntaxToken FirstOrDefault(this SyntaxTokenList list)
   {
      return list.Count == 0 ? default : list[0];
   }

   public static SyntaxToken FirstOrDefault(this SyntaxTokenList list, Func<SyntaxToken, bool> predicate)
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

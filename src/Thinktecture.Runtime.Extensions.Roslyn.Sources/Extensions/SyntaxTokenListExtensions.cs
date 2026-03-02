using System.Runtime.CompilerServices;

namespace Thinktecture;

public static class SyntaxTokenListExtensions
{
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static SyntaxToken FirstTokenOrDefault(this in SyntaxTokenList list)
   {
      return list.Count == 0 ? default : list[0];
   }
}

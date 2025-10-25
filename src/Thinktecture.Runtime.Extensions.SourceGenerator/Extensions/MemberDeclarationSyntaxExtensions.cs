using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture;

public static class MemberDeclarationSyntaxExtensions
{
   public static bool IsPartial(this MemberDeclarationSyntax tds)
   {
      if (tds is null)
         throw new ArgumentNullException(nameof(tds));

      for (var i = 0; i < tds.Modifiers.Count; i++)
      {
         if (tds.Modifiers[i].IsKind(SyntaxKind.PartialKeyword))
            return true;
      }

      return false;
   }
}

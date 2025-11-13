using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture;

public static class MemberDeclarationSyntaxExtensions
{
   public static bool IsPartial(this MemberDeclarationSyntax mds)
   {
      for (var i = 0; i < mds.Modifiers.Count; i++)
      {
         if (mds.Modifiers[i].IsKind(SyntaxKind.PartialKeyword))
            return true;
      }

      return false;
   }
}

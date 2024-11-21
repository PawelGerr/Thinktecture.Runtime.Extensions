using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture;

internal static class TypeDeclarationSyntaxExtensions
{
   public static bool IsPartial(this TypeDeclarationSyntax tds)
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

   public static bool IsGeneric(this TypeDeclarationSyntax tds)
   {
      if (tds is null)
         throw new ArgumentNullException(nameof(tds));

      return tds.TypeParameterList is { Parameters.Count: > 0 };
   }
}

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture;

internal static class TypeDeclarationSyntaxExtensions
{
   public static bool IsGeneric(this TypeDeclarationSyntax tds)
   {
      if (tds is null)
         throw new ArgumentNullException(nameof(tds));

      return tds.TypeParameterList is { Parameters.Count: > 0 };
   }
}

using Thinktecture.CodeAnalysis;

namespace Thinktecture;

public static class PropertyDeclarationSyntaxKindExtensions
{
   public static bool HasKind(this PropertyDeclarationSyntaxKind kind, PropertyDeclarationSyntaxKind kindToCheckFor)
   {
      return (kind & kindToCheckFor) == kindToCheckFor;
   }
}

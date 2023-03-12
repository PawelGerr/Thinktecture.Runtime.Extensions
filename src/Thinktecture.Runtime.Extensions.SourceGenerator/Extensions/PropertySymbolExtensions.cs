using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture;

public static class PropertySymbolExtensions
{
   public static SyntaxToken? GetIdentifier(this IPropertySymbol property, CancellationToken cancellationToken)
   {
      if (property.DeclaringSyntaxReferences.IsDefaultOrEmpty)
         return null;

      if (property.DeclaringSyntaxReferences[0].GetSyntax(cancellationToken) is PropertyDeclarationSyntax propertyDeclaration)
         return propertyDeclaration.Identifier;

      return null;
   }
}

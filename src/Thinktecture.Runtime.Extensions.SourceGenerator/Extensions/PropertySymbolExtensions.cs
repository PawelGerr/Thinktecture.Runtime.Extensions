using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture;

public static class PropertySymbolExtensions
{
   public static SyntaxToken GetIdentifier(this IPropertySymbol property, CancellationToken cancellationToken)
   {
      var syntax = (PropertyDeclarationSyntax)property.DeclaringSyntaxReferences.Single().GetSyntax(cancellationToken);
      return syntax.Identifier;
   }
}

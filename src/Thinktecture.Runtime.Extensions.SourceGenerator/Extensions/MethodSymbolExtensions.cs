using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture;

public static class MethodSymbolExtensions
{
   public static SyntaxToken GetIdentifier(this IMethodSymbol method, CancellationToken cancellationToken)
   {
      var syntax = (MethodDeclarationSyntax)method.DeclaringSyntaxReferences.Single().GetSyntax(cancellationToken);
      return syntax.Identifier;
   }
}

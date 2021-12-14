using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture;

public static class MethodSymbolExtensions
{
   public static SyntaxToken GetIdentifier(this IMethodSymbol method)
   {
      var syntax = (MethodDeclarationSyntax)method.DeclaringSyntaxReferences.Single().GetSyntax();
      return syntax.Identifier;
   }
}

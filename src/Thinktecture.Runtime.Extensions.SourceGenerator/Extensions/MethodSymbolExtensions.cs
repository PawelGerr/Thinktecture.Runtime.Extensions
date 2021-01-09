using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

// ReSharper disable once CheckNamespace
namespace Thinktecture
{
   public static class MethodSymbolExtensions
   {
      public static SyntaxToken GetIdentifier(this IMethodSymbol method)
      {
         var syntax = (MethodDeclarationSyntax)method.DeclaringSyntaxReferences.First().GetSyntax();
         return syntax.Identifier;
      }
   }
}

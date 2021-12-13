using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

// ReSharper disable once CheckNamespace
namespace Thinktecture;

public static class PropertySymbolExtensions
{
   public static SyntaxToken GetIdentifier(this IPropertySymbol property)
   {
      var syntax = (PropertyDeclarationSyntax)property.DeclaringSyntaxReferences.Single().GetSyntax();
      return syntax.Identifier;
   }
}
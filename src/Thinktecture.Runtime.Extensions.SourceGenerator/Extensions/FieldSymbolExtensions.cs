using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

// ReSharper disable once CheckNamespace
namespace Thinktecture
{
   public static class FieldSymbolExtensions
   {
      public static SyntaxToken GetIdentifier(this IFieldSymbol field)
      {
         var syntax = (VariableDeclaratorSyntax)field.DeclaringSyntaxReferences.First().GetSyntax();
         return syntax.Identifier;
      }
   }
}

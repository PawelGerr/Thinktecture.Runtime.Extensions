using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

// ReSharper disable once CheckNamespace
namespace Thinktecture;

public static class FieldSymbolExtensions
{
   public static SyntaxToken GetIdentifier(this IFieldSymbol field)
   {
      var syntax = (VariableDeclaratorSyntax)field.DeclaringSyntaxReferences.Single().GetSyntax();
      return syntax.Identifier;
   }

   public static bool IsPropertyBackingField(this IFieldSymbol field, [MaybeNullWhen(false)] out IPropertySymbol property)
   {
      if (field.AssociatedSymbol is IPropertySymbol prop)
      {
         property = prop;
         return true;
      }

      property = null;
      return false;
   }
}
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture;

public static class FieldSymbolExtensions
{
   public static SyntaxToken? GetIdentifier(this IFieldSymbol field, CancellationToken cancellationToken)
   {
      if (field.DeclaringSyntaxReferences.IsDefaultOrEmpty)
         return null;

      if (field.DeclaringSyntaxReferences[0].GetSyntax(cancellationToken) is VariableDeclaratorSyntax variableDeclaration)
         return variableDeclaration.Identifier;

      return null;
   }

   public static bool IsPropertyBackingField(this IFieldSymbol field)
   {
      return field.AssociatedSymbol is IPropertySymbol;
   }
}

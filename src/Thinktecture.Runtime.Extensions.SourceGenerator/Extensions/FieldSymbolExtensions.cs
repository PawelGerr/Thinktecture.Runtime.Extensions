using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture;

public static class FieldSymbolExtensions
{
   public static SyntaxToken? GetIdentifierSyntax(this IFieldSymbol field, CancellationToken cancellationToken)
   {
      if (field.DeclaringSyntaxReferences.IsDefaultOrEmpty)
         return null;

      for (var i = 0; i < field.DeclaringSyntaxReferences.Length; i++)
      {
         var syntaxReference = field.DeclaringSyntaxReferences[i];

         if (syntaxReference.GetSyntax(cancellationToken) is VariableDeclaratorSyntax variableDeclaration)
            return variableDeclaration.Identifier;
      }

      return null;
   }

   public static bool IsPropertyBackingField(this IFieldSymbol field)
   {
      return field is { IsImplicitlyDeclared: true, AssociatedSymbol: IPropertySymbol };
   }
}

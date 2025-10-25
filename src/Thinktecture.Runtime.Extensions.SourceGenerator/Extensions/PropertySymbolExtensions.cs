using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture;

public static class PropertySymbolExtensions
{
   /// <summary>
   /// Gets the identifier token for the property.
   /// Handles both regular property declarations and record positional parameters (synthesized properties).
   /// </summary>
   /// <param name="property">The property symbol.</param>
   /// <param name="cancellationToken">The cancellation token.</param>
   /// <returns>
   /// The identifier token if found; otherwise, <c>null</c>.
   /// Returns <c>null</c> for indexers (which use <see cref="IndexerDeclarationSyntax"/>).
   /// </returns>
   public static SyntaxToken? GetIdentifierSyntax(this IPropertySymbol property, CancellationToken cancellationToken)
   {
      if (property.DeclaringSyntaxReferences.IsDefaultOrEmpty)
         return null;

      for (var i = 0; i < property.DeclaringSyntaxReferences.Length; i++)
      {
         var syntaxReference = property.DeclaringSyntaxReferences[i];
         var syntax = syntaxReference.GetSyntax(cancellationToken);

         switch (syntax)
         {
            // Handle regular property declarations
            case PropertyDeclarationSyntax propertyDeclaration:
               return propertyDeclaration.Identifier;

            // Handle record positional parameters (synthesized properties)
            case ParameterSyntax parameter:
               return parameter.Identifier;
         }
      }

      return null;
   }
}

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Thinktecture.CodeAnalysis;

namespace Thinktecture;

public static class PropertySymbolExtensions
{
   public static Location GetPropertyLocation(
      this IPropertySymbol property,
      PropertyDeclarationSyntaxKind preferredKind,
      CancellationToken cancellationToken)
   {
      var syntaxReferences = property.DeclaringSyntaxReferences;

      if (syntaxReferences.IsDefaultOrEmpty)
         return Location.None;

      Location? implementationLocation = null;
      Location? declarationOnlyLocation = null;
      Location? inSourceLocation = null;
      Location? fallbackLocation = null;

      for (var i = 0; i < syntaxReferences.Length; i++)
      {
         var node = syntaxReferences[i].GetSyntax(cancellationToken);
         var tree = node.SyntaxTree;

         if (tree.IsGeneratedTree(cancellationToken))
            continue;

         Location? location;
         bool? hasImplementation = null;

         switch (node)
         {
            case PropertyDeclarationSyntax propertyDeclaration:
               location = propertyDeclaration.Identifier.GetLocation();

               if (!propertyDeclaration.IsPartial())
                  return location;

               // Prefer declaration that actually implements something (expression-bodied or accessor body)
               hasImplementation = propertyDeclaration.ExpressionBody is not null
                                   || (propertyDeclaration.AccessorList?.Accessors.Any(a => a.Body is not null || a.ExpressionBody is not null) ?? false);
               break;

            case IndexerDeclarationSyntax indexerDeclaration:
               // For indexers, use the 'this' keyword as the anchor
               location = indexerDeclaration.ThisKeyword.GetLocation();

               if (!indexerDeclaration.IsPartial())
                  return location;

               hasImplementation = indexerDeclaration.ExpressionBody is not null
                                   || (indexerDeclaration.AccessorList?.Accessors.Any(a => a.Body is not null || a.ExpressionBody is not null) ?? false);
               break;

            case ParameterSyntax parameterSyntax:
               // Positional record parameter that becomes a property
               return parameterSyntax.Identifier.GetLocation();

            default:
               // Fallback: some other syntax node associated with the property
               location = node.GetLocation();
               break;
         }

         switch (hasImplementation)
         {
            case true:
            {
               if (preferredKind.HasKind(PropertyDeclarationSyntaxKind.Implementation))
                  return location;

               implementationLocation ??= location;
               break;
            }
            case false:
            {
               if (preferredKind.HasFlag(PropertyDeclarationSyntaxKind.DeclarationOnly))
                  return location;

               declarationOnlyLocation ??= location;
               break;
            }
            default:
            {
               if (location.IsInSource)
               {
                  inSourceLocation ??= location;
               }
               else
               {
                  fallbackLocation ??= location;
               }

               break;
            }
         }
      }

      return implementationLocation ?? declarationOnlyLocation ?? inSourceLocation ?? fallbackLocation ?? Location.None;
   }
}

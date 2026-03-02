using Microsoft.CodeAnalysis.CSharp.Syntax;
using Thinktecture.CodeAnalysis;

namespace Thinktecture;

public static class FieldSymbolExtensions
{
   public static Location GetFieldLocation(this IFieldSymbol field, CancellationToken cancellationToken)
   {
      // 1) Synthesized/backing fields: anchor to associated symbol or containing type
      if (field.IsImplicitlyDeclared)
      {
         switch (field.AssociatedSymbol)
         {
            case IPropertySymbol property:
               // Prefer the property’s identifier location in user code
               return property.GetPropertyLocation(PropertyDeclarationSyntaxKind.All, cancellationToken);

            case IEventSymbol eventSymbol:
               // Prefer the event’s identifier location in user code
               return eventSymbol.GetEventLocation(cancellationToken);
         }

         // Enum underlying field "value__" and other synthesized fields: use containing type
         return field.ContainingType.GetTypeIdentifierLocation(cancellationToken);
      }

      // 2) Prefer the exact declarator/identifier in a non-generated tree
      Location? inSourceLocation = null;
      Location? fallbackLocation = null;

      for (var i = 0; i < field.DeclaringSyntaxReferences.Length; i++)
      {
         var node = field.DeclaringSyntaxReferences[i].GetSyntax(cancellationToken);

         if (node.SyntaxTree is not { } tree || tree.IsGeneratedTree(cancellationToken))
            continue;

         switch (node)
         {
            case VariableDeclaratorSyntax vds:
               return vds.Identifier.GetLocation();

            case EnumMemberDeclarationSyntax emds:
               return emds.Identifier.GetLocation();

            // Rare, but handle conservatively
            case FieldDeclarationSyntax fds:
            {
               // When the reference points to the FieldDeclaration, find the matching declarator
               for (var j = 0; j < fds.Declaration.Variables.Count; j++)
               {
                  var variable = fds.Declaration.Variables[j];

                  if (StringComparer.Ordinal.Equals(variable.Identifier.ValueText, field.Name))
                     return variable.Identifier.GetLocation();
               }

               // Fallback to whole declaration location
               return fds.GetLocation();
            }
            default:
            {
               var location = node.GetLocation();

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

      return inSourceLocation ?? fallbackLocation ?? Location.None;
   }
}

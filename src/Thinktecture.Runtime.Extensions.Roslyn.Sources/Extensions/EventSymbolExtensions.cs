using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture;

public static class EventSymbolExtensions
{
   public static Location GetEventLocation(this IEventSymbol eventSymbol, CancellationToken cancellationToken)
   {
      Location? inSourceLocation = null;
      Location? fallbackLocation = null;

      for (var i = 0; i < eventSymbol.DeclaringSyntaxReferences.Length; i++)
      {
         var node = eventSymbol.DeclaringSyntaxReferences[i].GetSyntax(cancellationToken);

         if (node.SyntaxTree is not { } tree || tree.IsGeneratedTree(cancellationToken))
            continue;

         switch (node)
         {
            case EventDeclarationSyntax eds:
               return eds.Identifier.GetLocation();

            case VariableDeclaratorSyntax vds:
               // event field syntax: event T E1, E2;
               return vds.Identifier.GetLocation();

            case EventFieldDeclarationSyntax efds:
            {
               for (var j = 0; j < efds.Declaration.Variables.Count; j++)
               {
                  var variableSyntax = efds.Declaration.Variables[j];

                  if (StringComparer.Ordinal.Equals(variableSyntax.Identifier.ValueText, eventSymbol.Name))
                     return variableSyntax.Identifier.GetLocation();
               }

               return efds.GetLocation();
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

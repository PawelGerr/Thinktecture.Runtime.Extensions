using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace Thinktecture;

public static class AttributeOperationExtensions
{
   public static Location GetTypeDeclarationLocationFromAttribute(this IAttributeOperation attributeOperation)
   {
      var attrSyntax = (AttributeSyntax)attributeOperation.Syntax;
      var tds = attrSyntax.FirstAncestorOrSelf<TypeDeclarationSyntax>();

      return tds?.Identifier.GetLocation() ?? attrSyntax.GetLocation();
   }
}

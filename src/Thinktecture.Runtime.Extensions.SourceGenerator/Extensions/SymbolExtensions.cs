using Microsoft.CodeAnalysis;

namespace Thinktecture;

public static class SymbolExtensions
{
   public static AttributeData? FindAttribute(this ISymbol type, Func<ITypeSymbol, bool> predicate)
   {
      var attributes = type.GetAttributes();

      if (attributes.IsDefaultOrEmpty)
         return null;

      for (var i = 0; i < attributes.Length; i++)
      {
         var attribute = attributes[i];

         if (attribute.AttributeClass is { } attributeClass && attributeClass.TypeKind != TypeKind.Error && predicate(attributeClass))
            return attribute;
      }

      return null;
   }

   public static bool HasAttribute(this ISymbol symbol, Func<ITypeSymbol, bool> predicate)
   {
      return symbol.FindAttribute(predicate) is not null;
   }
}

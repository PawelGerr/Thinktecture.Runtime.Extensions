using Microsoft.CodeAnalysis;

namespace Thinktecture;

public static class SymbolExtensions
{
   public static AttributeData? FindAttribute(this ISymbol type, string attributeType)
   {
      return type.GetAttributes().FirstOrDefault(a => a.AttributeClass?.ToString() == attributeType);
   }

   public static bool HasAttribute(this ISymbol symbol, string attributeType)
   {
      return symbol.FindAttribute(attributeType) is not null;
   }

   public static AttributeData? FindValueObjectConstructorAttribute(this ISymbol symbol)
   {
      return symbol.FindAttribute("Thinktecture.Internal.ValueObjectConstructorAttribute");
   }
}

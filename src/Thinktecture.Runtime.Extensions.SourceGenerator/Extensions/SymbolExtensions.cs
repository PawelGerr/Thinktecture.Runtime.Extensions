using System;
using System.Linq;
using Microsoft.CodeAnalysis;

// ReSharper disable once CheckNamespace
namespace Thinktecture
{
   public static class SymbolExtensions
   {
      public static bool IsString(this ITypeSymbol symbol)
      {
         if (symbol is null)
            throw new ArgumentNullException(nameof(symbol));

         return symbol.SpecialType == SpecialType.System_String;
      }

      public static AttributeData? FindAttribute(this ISymbol type, string attributeType)
      {
         return type.GetAttributes().FirstOrDefault(a => a.AttributeClass?.ToString() == attributeType);
      }

      public static bool HasAttribute(this ISymbol symbol, string attributeType)
      {
         return symbol.FindAttribute(attributeType) is not null;
      }

      public static AttributeData? FindValueTypeEqualityMemberAttribute(this ISymbol symbol)
      {
         return symbol.FindAttribute("Thinktecture.ValueTypeEqualityMemberAttribute");
      }

      public static AttributeData? FindEnumGenerationMemberAttribute(this ISymbol symbol)
      {
         return symbol.FindAttribute("Thinktecture.EnumGenerationMemberAttribute");
      }

      public static AttributeData? FindValueTypeConstructorAttribute(this ISymbol symbol)
      {
         return symbol.FindAttribute("Thinktecture.Internal.ValueTypeConstructorAttribute");
      }
   }
}

using System.Diagnostics.CodeAnalysis;
using Thinktecture.CodeAnalysis;

namespace Thinktecture;

public static class SymbolExtensions
{
   public static AttributeData? FindAttribute(this ISymbol type, Func<INamedTypeSymbol, bool> predicate)
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

   public static bool HasAttribute(this ISymbol symbol, Func<INamedTypeSymbol, bool> predicate)
   {
      return symbol.FindAttribute(predicate) is not null;
   }

   public static bool IsIgnored(this ISymbol member)
   {
      return member.HasAttribute(static attrType => attrType.IsValueObjectMemberIgnoreAttribute());
   }

   public static bool IsValidateFactoryArgumentsImplementation(
      this ISymbol member,
      [MaybeNullWhen(false)] out IMethodSymbol method)
   {
      if (member is { IsStatic: true, Name: Constants.Methods.VALIDATE_FACTORY_ARGUMENTS } and IMethodSymbol methodSymbol)
      {
         method = methodSymbol;
         return true;
      }

      method = null;
      return false;
   }
}

using Microsoft.CodeAnalysis;

namespace Thinktecture;

public static class SymbolExtensions
{
   public static AttributeData? FindAttribute(this ISymbol type, Func<ITypeSymbol, bool> predicate)
   {
      return type.GetAttributes().FirstOrDefault(a => a.AttributeClass is not null && predicate(a.AttributeClass));
   }

   public static bool HasAttribute(this ISymbol symbol, Func<ITypeSymbol, bool> predicate)
   {
      return symbol.FindAttribute(predicate) is not null;
   }

   public static AttributeData? FindValueObjectConstructorAttribute(this ISymbol symbol)
   {
      return symbol.FindAttribute(static type => type.Name == "ValueObjectConstructorAttribute"
                                                 && type.ContainingNamespace is
                                                 {
                                                    Name: "Internal",
                                                    ContainingNamespace: { Name: "Thinktecture", ContainingNamespace.IsGlobalNamespace: true }
                                                 });
   }
}

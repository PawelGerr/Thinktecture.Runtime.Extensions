using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

public class DefaultSymbolState : ISymbolState
{
   public string Identifier { get; }
   public ITypeSymbol Type { get; }
   public string ArgumentName { get; }
   public bool IsStatic { get; }

   public DefaultSymbolState(string identifier, ITypeSymbol type, string argumentName, bool isStatic)
   {
      Identifier = identifier;
      Type = type;
      ArgumentName = argumentName;
      IsStatic = isStatic;
   }

   public static DefaultSymbolState CreateFrom(IFieldSymbol field)
   {
      return new(field.Name, field.Type, field.Name.MakeArgumentName(), field.IsStatic);
   }

   public static DefaultSymbolState CreateFrom(IPropertySymbol property)
   {
      return new(property.Name, property.Type, property.Name.MakeArgumentName(), property.IsStatic);
   }
}
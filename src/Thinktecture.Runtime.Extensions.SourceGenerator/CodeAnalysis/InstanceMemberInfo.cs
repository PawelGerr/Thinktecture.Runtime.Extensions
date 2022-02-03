using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

public class InstanceMemberInfo : ISymbolState
{
   public ISymbol Symbol { get; }

   public ITypeSymbol Type { get; }
   public string TypeFullyQualified { get; }
   public string TypeMinimallyQualified { get; }

   public SyntaxToken Identifier { get; }
   public Accessibility ReadAccessibility { get; }
   public string ArgumentName { get; }
   public bool IsStatic { get; }
   public bool IsReferenceTypeOrNullableStruct => Type.IsReferenceType || Type.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T;
   public bool IsNullableStruct => Type.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T;
   public string? NullableQuestionMark => Type.IsReferenceType ? "?" : null;

   string ISymbolState.Identifier => Identifier.ToString();

   public InstanceMemberInfo(
      ISymbol symbol,
      ITypeSymbol type,
      SyntaxToken identifier,
      Accessibility readAccessibility,
      bool isStatic)
   {
      Symbol = symbol;
      Type = type;
      TypeFullyQualified = Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
      TypeMinimallyQualified = Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
      Identifier = identifier;
      ReadAccessibility = readAccessibility;
      IsStatic = isStatic;
      ArgumentName = identifier.Text.MakeArgumentName();
   }

   public static InstanceMemberInfo CreateFrom(IFieldSymbol isStatic)
   {
      return new(isStatic, isStatic.Type, isStatic.GetIdentifier(), isStatic.DeclaredAccessibility, isStatic.IsStatic);
   }

   public static InstanceMemberInfo CreateFrom(IPropertySymbol property)
   {
      return new(property, property.Type, property.GetIdentifier(), property.DeclaredAccessibility, property.IsStatic);
   }
}

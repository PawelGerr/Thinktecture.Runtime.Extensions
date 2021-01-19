using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis
{
   public class InstanceMemberInfo
   {
      public ISymbol Symbol { get; }
      public ITypeSymbol Type { get; }
      public SyntaxToken Identifier { get; }
      public string ArgumentName { get; }
      public bool IsReferenceTypeOrNullableStruct => Type.IsReferenceType || Type.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T;
      public bool IsNullableStruct => Type.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T;
      public string? NullableQuestionMark => Type.IsReferenceType ? "?" : null;

      public InstanceMemberInfo(ISymbol symbol, ITypeSymbol type, SyntaxToken identifier)
      {
         Symbol = symbol;
         Type = type;
         Identifier = identifier;
         ArgumentName = identifier.Text.MakeArgumentName();
      }
   }
}

using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis
{
   public class InstanceMemberInfo
   {
      public ISymbol Symbol { get; }
      public ITypeSymbol Type { get; }
      public SyntaxToken Identifier { get; }
      public string ArgumentName { get; }
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

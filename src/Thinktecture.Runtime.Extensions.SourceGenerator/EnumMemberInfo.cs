using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture
{
   public class EnumMemberInfo
   {
      public MemberDeclarationSyntax Syntax { get; }
      public ITypeSymbol Type { get; }
      public SyntaxToken Identifier { get; }
      public string ArgumentName { get; }

      public EnumMemberInfo(MemberDeclarationSyntax syntax, ITypeSymbol type, SyntaxToken identifier, string argumentName)
      {
         Syntax = syntax;
         Type = type;
         Identifier = identifier;
         ArgumentName = argumentName;
      }
   }
}

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture
{
   public class EnumInterfaceInfo
   {
      public GenericNameSyntax Syntax { get; }
      public ITypeSymbol Type { get; }
      public ITypeSymbol KeyType { get; }
      public bool IsValidatable { get; }

      public EnumInterfaceInfo(
         GenericNameSyntax syntax,
         ITypeSymbol type,
         ITypeSymbol keyType,
         bool isValidatable)
      {
         Syntax = syntax;
         Type = type;
         KeyType = keyType;
         IsValidatable = isValidatable;
      }
   }
}

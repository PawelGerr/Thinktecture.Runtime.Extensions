using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture
{
   public class EnumMemberInfo
   {
      public ISymbol Symbol { get; }
      public ITypeSymbol Type { get; }
      public string ArgumentName { get; }

      public EnumMemberInfo(ISymbol symbol, ITypeSymbol type)
      {
         Symbol = symbol;
         Type = type;
         ArgumentName = symbol.Name.MakeArgumentName();
      }
   }
}

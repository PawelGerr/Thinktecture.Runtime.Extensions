using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis
{
   public class InstanceMemberInfo
   {
      public ISymbol Symbol { get; }
      public ITypeSymbol Type { get; }
      public string ArgumentName { get; }

      public InstanceMemberInfo(ISymbol symbol, ITypeSymbol type)
      {
         Symbol = symbol;
         Type = type;
         ArgumentName = symbol.Name.MakeArgumentName();
      }
   }
}

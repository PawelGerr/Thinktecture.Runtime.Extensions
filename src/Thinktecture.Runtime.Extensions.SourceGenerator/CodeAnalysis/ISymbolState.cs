using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis
{
   public interface ISymbolState
   {
      string Identifier { get; }
      ITypeSymbol Type { get; }
      string ArgumentName { get; }
      bool IsStatic { get; }
   }
}

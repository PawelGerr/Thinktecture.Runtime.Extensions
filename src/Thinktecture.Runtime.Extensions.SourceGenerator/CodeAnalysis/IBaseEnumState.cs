using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

public interface IBaseEnumState
{
   bool IsSameAssembly { get; }
   INamedTypeSymbol Type { get; }
   string? NullableQuestionMark { get; }
   IReadOnlyList<ISymbolState> Items { get; }
   IReadOnlyList<ISymbolState> ConstructorArguments { get; }
}
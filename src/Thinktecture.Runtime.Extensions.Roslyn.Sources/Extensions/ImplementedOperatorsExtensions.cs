using System.Runtime.CompilerServices;
using Thinktecture.CodeAnalysis;

namespace Thinktecture;

public static class ImplementedOperatorsExtensions
{
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static bool HasOperator(this ImplementedOperators operators, ImplementedOperators operatorToCheckFor)
   {
      return operatorToCheckFor != 0 && (operators & operatorToCheckFor) == operatorToCheckFor;
   }
}

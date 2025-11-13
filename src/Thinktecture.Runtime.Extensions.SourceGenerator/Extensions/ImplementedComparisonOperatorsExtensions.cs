using System.Runtime.CompilerServices;
using Thinktecture.CodeAnalysis;

namespace Thinktecture;

public static class ImplementedComparisonOperatorsExtensions
{
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static bool HasOperator(this ImplementedComparisonOperators operators, ImplementedComparisonOperators operatorToCheckFor)
   {
      return operatorToCheckFor != 0 && (operators & operatorToCheckFor) == operatorToCheckFor;
   }
}

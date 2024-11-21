using Thinktecture.CodeAnalysis;

namespace Thinktecture;

public static class ImplementedComparisonOperatorsExtensions
{
   public static bool HasOperator(this ImplementedComparisonOperators operators, ImplementedComparisonOperators operatorToCheckFor)
   {
      return (operators & operatorToCheckFor) == operatorToCheckFor;
   }
}

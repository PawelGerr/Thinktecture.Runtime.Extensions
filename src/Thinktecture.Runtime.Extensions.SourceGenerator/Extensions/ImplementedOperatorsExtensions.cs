using Thinktecture.CodeAnalysis;

namespace Thinktecture;

public static class ImplementedOperatorsExtensions
{
   public static bool HasOperator(this ImplementedOperators operators, ImplementedOperators operatorToCheckFor)
   {
      return (operators & operatorToCheckFor) == operatorToCheckFor;
   }
}

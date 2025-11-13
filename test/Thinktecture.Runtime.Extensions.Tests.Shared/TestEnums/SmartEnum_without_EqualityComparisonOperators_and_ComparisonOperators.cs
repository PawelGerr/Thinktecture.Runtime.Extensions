namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
[SmartEnum<int>(EqualityComparisonOperators = OperatorsGeneration.None, ComparisonOperators = OperatorsGeneration.None)]
public partial class SmartEnum_without_EqualityComparisonOperators_and_ComparisonOperators
{
   public static readonly SmartEnum_without_EqualityComparisonOperators_and_ComparisonOperators Item1 = new(1);
   public static readonly SmartEnum_without_EqualityComparisonOperators_and_ComparisonOperators Item2 = new(2);
}

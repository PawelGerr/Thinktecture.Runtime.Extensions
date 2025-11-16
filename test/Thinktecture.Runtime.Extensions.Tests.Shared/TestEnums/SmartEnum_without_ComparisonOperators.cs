namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
[SmartEnum<int>(
   ComparisonOperators = OperatorsGeneration.None,
   EqualityComparisonOperators = OperatorsGeneration.None)]
public partial class SmartEnum_without_ComparisonOperators
{
   public static readonly SmartEnum_without_ComparisonOperators Item1 = new(1);
   public static readonly SmartEnum_without_ComparisonOperators Item2 = new(2);
}

namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
[SmartEnum<int>(EqualityComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads)]
public partial class SmartEnum_with_EqualityComparisonOperators_DefaultWithKeyTypeOverloads
{
   public static readonly SmartEnum_with_EqualityComparisonOperators_DefaultWithKeyTypeOverloads Item1 = new(1);
   public static readonly SmartEnum_with_EqualityComparisonOperators_DefaultWithKeyTypeOverloads Item2 = new(2);
}

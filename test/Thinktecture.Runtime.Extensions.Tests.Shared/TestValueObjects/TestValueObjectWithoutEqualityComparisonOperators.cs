namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ValueObject<int>(
   EqualityComparisonOperators = OperatorsGeneration.None,
   ComparisonOperators = OperatorsGeneration.None)]
public partial class TestValueObjectWithoutEqualityComparisonOperators;

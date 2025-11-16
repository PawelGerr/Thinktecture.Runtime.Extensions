namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ValueObject<int>(
   ComparisonOperators = OperatorsGeneration.None,
   EqualityComparisonOperators = OperatorsGeneration.None)]
public partial class TestValueObjectWithoutComparisonOperators;

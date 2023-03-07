namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ValueObject(ComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
             AdditionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
             SubtractionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
             MultiplyOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
             DivisionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads)]
public sealed partial class DecimalBasedClassValueObject
{
   public decimal Property { get; }
}

namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ValueObject(CreateFactoryMethodName = "Get",
             TryCreateFactoryMethodName = "TryGet",
             AdditionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
             ComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
             DivisionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
             MultiplyOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
             SubtractionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads)]
public sealed partial class IntBasedReferenceValueObjectWithCustomFactoryNames
{
   public int Property { get; }
}

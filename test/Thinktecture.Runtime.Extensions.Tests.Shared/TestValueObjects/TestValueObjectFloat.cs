namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ValueObject<float>(AdditionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                    SubtractionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                    MultiplyOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                    DivisionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads)]
public partial class TestValueObjectFloat;

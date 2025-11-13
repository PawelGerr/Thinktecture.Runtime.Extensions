namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ValueObject<ulong>(AdditionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                    SubtractionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                    MultiplyOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                    DivisionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads)]
public partial class TestValueObjectULong;

namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ValueObject<double>(AdditionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                     SubtractionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                     MultiplyOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                     DivisionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads)]
public partial class TestValueObjectDouble;

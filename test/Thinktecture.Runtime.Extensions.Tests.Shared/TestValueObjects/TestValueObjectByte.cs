namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ValueObject<byte>(AdditionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                   SubtractionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                   MultiplyOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                   DivisionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads)]
public partial class TestValueObjectByte;

[ValueObject<sbyte>(AdditionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                    SubtractionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                    MultiplyOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                    DivisionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads)]
public partial class TestValueObjectSByte;

[ValueObject<short>(AdditionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                    SubtractionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                    MultiplyOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                    DivisionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads)]
public partial class TestValueObjectShort;

[ValueObject<ushort>(AdditionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                     SubtractionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                     MultiplyOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                     DivisionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads)]
public partial class TestValueObjectUShort;

[ValueObject<int>(AdditionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                  SubtractionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                  MultiplyOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                  DivisionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads)]
public partial class TestValueObjectInt;

[ValueObject<uint>(AdditionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                   SubtractionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                   MultiplyOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                   DivisionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads)]
public partial class TestValueObjectUInt;

[ValueObject<long>(AdditionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                   SubtractionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                   MultiplyOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                   DivisionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads)]
public partial class TestValueObjectLong;

[ValueObject<ulong>(AdditionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                    SubtractionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                    MultiplyOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                    DivisionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads)]
public partial class TestValueObjectULong;

[ValueObject<float>(AdditionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                    SubtractionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                    MultiplyOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                    DivisionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads)]
public partial class TestValueObjectFloat;

[ValueObject<double>(AdditionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                     SubtractionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                     MultiplyOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                     DivisionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads)]
public partial class TestValueObjectDouble;

[ValueObject<decimal>(AdditionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                      SubtractionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                      MultiplyOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                      DivisionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads)]
public partial class TestValueObjectDecimal;

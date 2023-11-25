namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ValueObject<decimal>(KeyMemberKind = ValueObjectMemberKind.Property,
                      KeyMemberName = "Property",
                      KeyMemberAccessModifier = ValueObjectAccessModifier.Public,
                      ComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                      AdditionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                      SubtractionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                      MultiplyOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                      DivisionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads)]
public sealed partial class DecimalBasedClassValueObject
{
}

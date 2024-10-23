namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ValueObject<int>(KeyMemberKind = ValueObjectMemberKind.Property,
                  KeyMemberName = "Property",
                  KeyMemberAccessModifier = ValueObjectAccessModifier.Public,
                  EqualityComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                  ComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads)]
public readonly partial struct IntBasedStructValueObject
{
}

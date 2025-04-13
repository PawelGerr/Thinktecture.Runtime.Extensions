namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ValueObject<int>(KeyMemberKind = ValueObjectMemberKind.Property,
                  KeyMemberName = "Property",
                  KeyMemberAccessModifier = ValueObjectAccessModifier.Public,
                  AllowDefaultStructs = true,
                  EqualityComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                  ComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads)]
public partial struct IntBasedStructValueObject;

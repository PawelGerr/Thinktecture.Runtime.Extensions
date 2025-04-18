namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ValueObject<int>(KeyMemberKind = MemberKind.Property,
                  KeyMemberName = "Property",
                  KeyMemberAccessModifier = AccessModifier.Public,
                  AllowDefaultStructs = true,
                  EqualityComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                  ComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads)]
public partial struct IntBasedStructValueObject;

namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ValueObject<decimal>(KeyMemberKind = MemberKind.Property,
                      KeyMemberName = "Property",
                      KeyMemberAccessModifier = AccessModifier.Public,
                      ComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                      EqualityComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                      AdditionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                      SubtractionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                      MultiplyOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                      DivisionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads)]
public partial class DecimalBasedClassValueObject;

namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ValueObject<int>(KeyMemberKind = MemberKind.Property,
                  KeyMemberName = "Property",
                  KeyMemberAccessModifier = AccessModifier.Public,
                  CreateFactoryMethodName = "Get",
                  TryCreateFactoryMethodName = "TryGet",
                  AdditionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                  ComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                  DivisionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                  MultiplyOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                  SubtractionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads)]
public partial class IntBasedReferenceValueObjectWithCustomFactoryNames;

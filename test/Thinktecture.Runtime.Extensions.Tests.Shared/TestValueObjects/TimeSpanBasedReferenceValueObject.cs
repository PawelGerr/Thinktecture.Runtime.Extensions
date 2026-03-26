using System;

namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ValueObject<TimeSpan>(KeyMemberKind = MemberKind.Property,
                       KeyMemberName = "Property",
                       KeyMemberAccessModifier = AccessModifier.Public,
                       AdditionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                       SubtractionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                       MultiplyOperators = OperatorsGeneration.None,
                       DivisionOperators = OperatorsGeneration.None)]
public partial class TimeSpanBasedReferenceValueObject;

namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ValueObject<int>(KeyMemberKind = MemberKind.Property,
                  KeyMemberName = "Property",
                  KeyMemberAccessModifier = AccessModifier.Public)]
public partial class IntBasedReferenceValueObject;

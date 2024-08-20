namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ValueObject<int>(KeyMemberKind = ValueObjectMemberKind.Property,
                  KeyMemberName = "Property",
                  KeyMemberAccessModifier = ValueObjectAccessModifier.Public)]
public partial class IntBasedReferenceValueObject
{
}

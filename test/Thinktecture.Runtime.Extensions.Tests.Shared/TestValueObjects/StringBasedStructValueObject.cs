namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ValueObject<string>(KeyMemberKind = ValueObjectMemberKind.Property,
                     KeyMemberName = "Property",
                     KeyMemberAccessModifier = ValueObjectAccessModifier.Public)]
public partial struct StringBasedStructValueObject
{
}

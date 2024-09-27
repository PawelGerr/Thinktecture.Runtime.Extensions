namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ValueObject<string>(KeyMemberKind = ValueObjectMemberKind.Property,
                     KeyMemberName = "Property",
                     KeyMemberAccessModifier = ValueObjectAccessModifier.Public,
                     AllowDefaultStructs = true)]
public partial struct StringBasedStructValueObject
{
}

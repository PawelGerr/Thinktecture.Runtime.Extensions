namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ValueObject<string>(KeyMemberKind = ValueObjectMemberKind.Property,
                     KeyMemberName = "Property",
                     KeyMemberAccessModifier = ValueObjectAccessModifier.Public,
                     AllowDefaultStructs = true)]
[ValueObjectKeyMemberEqualityComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>]
[ValueObjectKeyMemberComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>]
public partial struct StringBasedStructValueObject;

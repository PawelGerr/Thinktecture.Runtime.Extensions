namespace Thinktecture.Runtime.Tests.TestValueObjects;

// Exposes the enum key as a public property so Swashbuckle walks it and registers the key enum as a
// (otherwise orphaned) component - mirrors the Smart Enum orphan scenario for keyed Value Objects.
[ValueObject<ValueObject_EnumKey>(KeyMemberKind = MemberKind.Property,
                                  KeyMemberName = "Key",
                                  KeyMemberAccessModifier = AccessModifier.Public)]
public partial class EnumBasedValueObjectWithPublicKey;

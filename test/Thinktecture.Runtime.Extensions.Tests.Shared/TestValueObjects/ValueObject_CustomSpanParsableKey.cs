using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.TestValueObjects;

// ReSharper disable once InconsistentNaming
[ValueObject<CustomSpanParsableKey>(KeyMemberKind = MemberKind.Property,
                                     KeyMemberName = "Value",
                                     KeyMemberAccessModifier = AccessModifier.Public)]
public partial class ValueObject_CustomSpanParsableKey
{
}

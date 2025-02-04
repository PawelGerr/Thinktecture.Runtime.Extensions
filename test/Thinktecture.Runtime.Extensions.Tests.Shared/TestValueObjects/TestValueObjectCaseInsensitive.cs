namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ValueObject<string>]
[ValueObjectKeyMemberEqualityComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>]
[ValueObjectKeyMemberComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>]
public partial class TestValueObjectCaseInsensitive
{
}

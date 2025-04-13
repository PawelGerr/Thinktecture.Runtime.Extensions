namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ValueObject<string>]
[ValueObjectKeyMemberEqualityComparer<ComparerAccessors.StringOrdinal, string>]
[ValueObjectKeyMemberComparer<ComparerAccessors.StringOrdinal, string>]
public partial class TestValueObjectCaseSensitive;

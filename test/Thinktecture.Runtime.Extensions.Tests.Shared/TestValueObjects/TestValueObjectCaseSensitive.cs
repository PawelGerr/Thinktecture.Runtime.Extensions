namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ValueObject<string>]
[KeyMemberEqualityComparer<ComparerAccessors.StringOrdinal, string>]
[KeyMemberComparer<ComparerAccessors.StringOrdinal, string>]
public partial class TestValueObjectCaseSensitive;

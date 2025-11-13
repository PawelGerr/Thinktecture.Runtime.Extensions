namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ValueObject<string>(SkipKeyMember = true, KeyMemberName = "_customValue")]
[KeyMemberEqualityComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>]
[KeyMemberComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>]
public partial class TestValueObjectCustomKeyMemberField
{
   private readonly string _customValue;
}

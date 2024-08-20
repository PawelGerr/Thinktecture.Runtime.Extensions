namespace Thinktecture.ValueObjects;

[ValueObject<string>]
[ValueObjectKeyMemberComparer<ComparerAccessors.StringOrdinal, string>]
[ValueObjectKeyMemberEqualityComparer<ComparerAccessors.StringOrdinal, string>]
public partial class ProductNameWithCaseSensitiveComparer
{
}

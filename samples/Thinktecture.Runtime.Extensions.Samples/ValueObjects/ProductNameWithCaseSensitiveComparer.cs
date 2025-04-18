namespace Thinktecture.ValueObjects;

[ValueObject<string>]
[KeyMemberComparer<ComparerAccessors.StringOrdinal, string>]
[KeyMemberEqualityComparer<ComparerAccessors.StringOrdinal, string>]
public partial class ProductNameWithCaseSensitiveComparer;

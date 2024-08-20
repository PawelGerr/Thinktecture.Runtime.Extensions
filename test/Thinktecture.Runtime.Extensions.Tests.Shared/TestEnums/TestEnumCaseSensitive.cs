namespace Thinktecture.Runtime.Tests.TestEnums;

[SmartEnum<string>]
[ValueObjectKeyMemberComparer<ComparerAccessors.StringOrdinal, string>]
[ValueObjectKeyMemberEqualityComparer<ComparerAccessors.StringOrdinal, string>]
public partial class TestEnumCaseSensitive
{
   public static readonly TestEnumCaseSensitive LowerCased = new("item");
   public static readonly TestEnumCaseSensitive UpperCased = new("ITEM");
   public static readonly TestEnumCaseSensitive LowerCased2 = new("item2");
}

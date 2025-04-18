namespace Thinktecture.Runtime.Tests.TestEnums;

[SmartEnum<string>(IsValidatable = true)]
[KeyMemberComparer<ComparerAccessors.StringOrdinal, string>]
[KeyMemberEqualityComparer<ComparerAccessors.StringOrdinal, string>]
public partial class ValidatableTestEnumCaseSensitive
{
   public static readonly ValidatableTestEnumCaseSensitive LowerCased = new("item");
   public static readonly ValidatableTestEnumCaseSensitive UpperCased = new("ITEM");
   public static readonly ValidatableTestEnumCaseSensitive LowerCased2 = new("item2");
}

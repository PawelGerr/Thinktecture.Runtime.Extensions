namespace Thinktecture.Runtime.Tests.TestEnums;

[SmartEnum<string>(IsValidatable = true)]
[ValueObjectKeyMemberComparer<ComparerAccessors.StringOrdinal, string>]
[ValueObjectKeyMemberEqualityComparer<ComparerAccessors.StringOrdinal, string>]
public sealed partial class ValidatableTestEnumCaseSensitive
{
   public static readonly ValidatableTestEnumCaseSensitive LowerCased = new("item");
   public static readonly ValidatableTestEnumCaseSensitive UpperCased = new("ITEM");
   public static readonly ValidatableTestEnumCaseSensitive LowerCased2 = new("item2");
}

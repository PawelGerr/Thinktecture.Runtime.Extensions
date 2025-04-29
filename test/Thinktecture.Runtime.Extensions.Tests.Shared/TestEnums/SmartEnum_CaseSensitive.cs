namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
[SmartEnum<string>]
[KeyMemberComparer<ComparerAccessors.StringOrdinal, string>]
[KeyMemberEqualityComparer<ComparerAccessors.StringOrdinal, string>]
public partial class SmartEnum_CaseSensitive
{
   public static readonly SmartEnum_CaseSensitive LowerCased = new("item");
   public static readonly SmartEnum_CaseSensitive UpperCased = new("ITEM");
   public static readonly SmartEnum_CaseSensitive LowerCased2 = new("item2");
}

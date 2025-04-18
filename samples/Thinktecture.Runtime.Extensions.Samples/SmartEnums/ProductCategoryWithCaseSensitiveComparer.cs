namespace Thinktecture.SmartEnums;

[SmartEnum<string>]
[KeyMemberComparer<ComparerAccessors.StringOrdinal, string>]
[KeyMemberEqualityComparer<ComparerAccessors.StringOrdinal, string>]
public partial class ProductCategoryWithCaseSensitiveComparer
{
   public static readonly ProductCategoryWithCaseSensitiveComparer FruitsLowerCased = new("fruits");
   public static readonly ProductCategoryWithCaseSensitiveComparer FruitsUpperCased = new("FRUITS");
}

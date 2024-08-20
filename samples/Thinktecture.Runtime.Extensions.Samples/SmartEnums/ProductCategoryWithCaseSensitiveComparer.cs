namespace Thinktecture.SmartEnums;

[SmartEnum<string>]
[ValueObjectKeyMemberComparer<ComparerAccessors.StringOrdinal, string>]
[ValueObjectKeyMemberEqualityComparer<ComparerAccessors.StringOrdinal, string>]
public partial class ProductCategoryWithCaseSensitiveComparer
{
   public static readonly ProductCategoryWithCaseSensitiveComparer FruitsLowerCased = new("fruits");
   public static readonly ProductCategoryWithCaseSensitiveComparer FruitsUpperCased = new("FRUITS");
}

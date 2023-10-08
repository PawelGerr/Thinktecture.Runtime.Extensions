namespace Thinktecture;

[SmartEnum<string>(IsValidatable = true)]
public sealed partial class ProductCategoryWithJsonConverter
{
   public static readonly ProductCategoryWithJsonConverter Fruits = new("Fruits");
   public static readonly ProductCategoryWithJsonConverter Dairy = new("Dairy");
}

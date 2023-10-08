namespace Thinktecture.SmartEnums;

[SmartEnum<string>(IsValidatable = true,
                   KeyPropertyName = "Name")]
public sealed partial class ProductCategory
{
   public static readonly ProductCategory Fruits = new("Fruits");
   public static readonly ProductCategory Dairy = new("Dairy");
}

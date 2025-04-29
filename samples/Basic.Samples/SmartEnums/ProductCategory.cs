namespace Thinktecture.SmartEnums;

[SmartEnum<string>(KeyMemberName = "Name")]
public partial class ProductCategory
{
   public static readonly ProductCategory Fruits = new("Fruits");
   public static readonly ProductCategory Dairy = new("Dairy");
}

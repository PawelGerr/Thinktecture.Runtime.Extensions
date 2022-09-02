namespace Thinktecture.SmartEnums;

public partial class ProductType : IEnum<string>
{
   public static readonly ProductType Groceries = new("Groceries");
   public static readonly ProductType Housewares = new("Housewares");
}

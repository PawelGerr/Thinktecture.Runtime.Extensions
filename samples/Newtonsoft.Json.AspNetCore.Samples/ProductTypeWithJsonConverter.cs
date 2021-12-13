namespace Thinktecture;

public sealed partial class ProductTypeWithJsonConverter : IEnum<string>
{
   public static readonly ProductTypeWithJsonConverter Groceries = new("Groceries");
   public static readonly ProductTypeWithJsonConverter Housewares = new("Housewares");
}
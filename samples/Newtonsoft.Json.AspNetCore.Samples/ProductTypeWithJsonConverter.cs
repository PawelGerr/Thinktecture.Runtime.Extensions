namespace Thinktecture;

[SmartEnum<string>]
public sealed partial class ProductTypeWithJsonConverter
{
   public static readonly ProductTypeWithJsonConverter Groceries = new("Groceries");
   public static readonly ProductTypeWithJsonConverter Housewares = new("Housewares");
}

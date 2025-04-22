namespace Thinktecture;

[SmartEnum<string>]
public partial class ProductTypeWithMessagePackFormatter
{
   public static readonly ProductTypeWithMessagePackFormatter Groceries = new("Groceries");
   public static readonly ProductTypeWithMessagePackFormatter Housewares = new("Housewares");
}

namespace Thinktecture;

[SmartEnum<string>]
public sealed partial class ProductTypeWithMessagePackFormatter
{
   public static readonly ProductTypeWithMessagePackFormatter Groceries = new("Groceries");
   public static readonly ProductTypeWithMessagePackFormatter Housewares = new("Housewares");
}

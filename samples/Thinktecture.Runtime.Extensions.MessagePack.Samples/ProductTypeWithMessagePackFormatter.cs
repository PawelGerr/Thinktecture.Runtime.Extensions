namespace Thinktecture;

public sealed partial class ProductTypeWithMessagePackFormatter : IEnum<string>
{
   public static readonly ProductTypeWithMessagePackFormatter Groceries = new("Groceries");
   public static readonly ProductTypeWithMessagePackFormatter Housewares = new("Housewares");
}

namespace Thinktecture.SmartEnums;

[SmartEnum<string>]
[ValueObjectValidationError<ProductTypeValidationError>]
public sealed partial class ProductType
{
   public static readonly ProductType Groceries = new("Groceries");
   public static readonly ProductType Housewares = new("Housewares");
}

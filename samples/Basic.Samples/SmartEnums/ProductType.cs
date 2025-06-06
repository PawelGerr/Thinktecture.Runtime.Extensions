namespace Thinktecture.SmartEnums;

[SmartEnum<string>(SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
                   MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
[ValidationError<ProductTypeValidationError>]
public partial class ProductType
{
   public static readonly ProductType Groceries = new("Groceries");
   public static readonly ProductType Housewares = new("Housewares");
}

namespace Thinktecture.SmartEnums;

/// <summary>
/// Represents a product type.
/// </summary>
[SmartEnum<string>(SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
                   MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
[ValidationError<ProductTypeValidationError>]
public partial class ProductType
{
   /// <summary>
   /// Product type for groceries.
   /// </summary>
   public static readonly ProductType Groceries = new("Groceries");

   /// <summary>
   /// Product type for housewares.
   /// </summary>
   public static readonly ProductType Housewares = new("Housewares");
}

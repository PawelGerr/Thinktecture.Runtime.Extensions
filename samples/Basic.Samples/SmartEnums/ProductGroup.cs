namespace Thinktecture.SmartEnums;

[SmartEnum<int>(
   ComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
   EqualityComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
   SkipToString = true)]
public partial class ProductGroup
{
   public static readonly ProductGroup Apple = new(1, "Apple", ProductCategory.Fruits);
   public static readonly ProductGroup Orange = new(2, "Orange", ProductCategory.Fruits);

   public string DisplayName { get; }
   public ProductCategory Category { get; }

   public int Do(string foo)
   {
      // do something

      return 42;
   }

   public override string ToString()
   {
      return DisplayName;
   }
}

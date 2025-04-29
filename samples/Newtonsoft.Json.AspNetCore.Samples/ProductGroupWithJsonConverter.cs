using Thinktecture.SmartEnums;

namespace Thinktecture;

[SmartEnum<int>]
public partial class ProductGroupWithJsonConverter
{
   public static readonly ProductGroupWithJsonConverter Apple = new(1, "Apple", ProductCategory.Fruits);
   public static readonly ProductGroupWithJsonConverter Orange = new(2, "Orange", ProductCategory.Fruits);

   public string DisplayName { get; }
   public ProductCategory Category { get; }

   public int Do(string foo)
   {
      // do something

      return 42;
   }
}

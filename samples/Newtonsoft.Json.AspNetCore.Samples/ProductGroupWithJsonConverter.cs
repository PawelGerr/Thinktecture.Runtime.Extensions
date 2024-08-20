using Thinktecture.SmartEnums;

namespace Thinktecture;

[SmartEnum<int>(IsValidatable = true)]
public partial class ProductGroupWithJsonConverter
{
   public static readonly ProductGroupWithJsonConverter Apple = new(1, "Apple", ProductCategory.Fruits);
   public static readonly ProductGroupWithJsonConverter Orange = new(2, "Orange", ProductCategory.Fruits);

   public string DisplayName { get; }
   public ProductCategory Category { get; }

   public int Do(string foo)
   {
      EnsureValid(); // "Do()" is not allowed for invalid items

      // do something

      return 42;
   }

   private static ProductGroupWithJsonConverter CreateInvalidItem(int key)
   {
      // the values can be anything besides the key,
      // the key must not be null
      return new(key, "Unknown product group", ProductCategory.Get("Unknown"));
   }
}

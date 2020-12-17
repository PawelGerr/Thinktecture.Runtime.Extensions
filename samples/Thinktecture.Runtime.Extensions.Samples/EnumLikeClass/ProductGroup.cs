namespace Thinktecture.EnumLikeClass
{
   public sealed partial class ProductGroup : IEnum<int>
   {
      public static readonly ProductGroup Apple = new(1, "Apple", ProductCategory.Fruits);
      public static readonly ProductGroup Orange = new(2, "Orange", ProductCategory.Fruits);

      public string DisplayName { get; }
      public ProductCategory Category { get; }

      private ProductGroup(int key, string displayName, ProductCategory category)
         : this(key)
      {
         DisplayName = displayName;
         Category = category;
      }

      public int Do(string foo)
      {
         EnsureValid(); // "Do()" is not allowed for invalid items

         // do something

         return 42;
      }

      private static ProductGroup CreateInvalidItem(int key)
      {
         // the values can be anything besides the key,
         // the key must not be null
         return new(key, "Unknown product group", ProductCategory.Get("Unknown"))
                {
                   IsValid = false
                };
      }
   }
}

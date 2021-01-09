namespace Thinktecture.EnumLikeClass
{
   public sealed partial class ProductGroup : IValidatableEnum<int>
   {
      public static readonly ProductGroup Apple = new(1, "Apple", ProductCategory.Fruits);
      public static readonly ProductGroup Orange = new(2, "Orange", ProductCategory.Fruits);

      public string DisplayName { get; }
      public ProductCategory Category { get; }

      public int Do(string foo)
      {
         EnsureValid(); // "Do()" is not allowed for invalid items

         // do something

         return 42;
      }

      static partial void ValidateConstructorArguments(int key, bool isValid, ref string displayName, ref ProductCategory category)
      {
      }

      private static ProductGroup CreateInvalidItem(int key)
      {
         // the values can be anything besides the key,
         // the key must not be null
         return new(key, false, "Unknown product group", ProductCategory.Get("Unknown"));
      }
   }
}

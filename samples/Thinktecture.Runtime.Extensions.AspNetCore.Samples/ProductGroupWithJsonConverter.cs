using System.Text.Json.Serialization;
using Thinktecture.EnumLikeClass;
using Thinktecture.Text.Json.Serialization;

namespace Thinktecture
{
   [JsonConverter(typeof(EnumJsonConverter<ProductGroupWithJsonConverter, int>))]
   public sealed partial class ProductGroupWithJsonConverter : IEnum<int>
   {
      public static readonly ProductGroupWithJsonConverter Apple = new(1, "Apple", ProductCategory.Fruits);
      public static readonly ProductGroupWithJsonConverter Orange = new(2, "Orange", ProductCategory.Fruits);

      public string DisplayName { get; }
      public ProductCategory Category { get; }

      private ProductGroupWithJsonConverter(int key, string displayName, ProductCategory category)
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

      private static ProductGroupWithJsonConverter CreateInvalid(int key)
      {
         // the values can be anything besides the key,
         // the key must not be null
         return new(key, "Unknown product group", ProductCategory.Get("Unknown"));
      }
   }
}

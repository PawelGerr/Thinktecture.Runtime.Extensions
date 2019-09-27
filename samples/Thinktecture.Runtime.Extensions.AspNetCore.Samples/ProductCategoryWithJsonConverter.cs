using System.Text.Json.Serialization;
using Thinktecture.Text.Json.Serialization;

namespace Thinktecture
{
   [JsonConverter(typeof(EnumJsonConverter<ProductCategoryWithJsonConverter>))]
   public sealed class ProductCategoryWithJsonConverter : Enum<ProductCategoryWithJsonConverter>
   {
      public static readonly ProductCategoryWithJsonConverter Fruits = new ProductCategoryWithJsonConverter("Fruits");
      public static readonly ProductCategoryWithJsonConverter Dairy = new ProductCategoryWithJsonConverter("Dairy");

      private ProductCategoryWithJsonConverter(string key)
         : base(key)
      {
      }

      protected override ProductCategoryWithJsonConverter CreateInvalid(string key)
      {
         return new ProductCategoryWithJsonConverter(key);
      }
   }
}

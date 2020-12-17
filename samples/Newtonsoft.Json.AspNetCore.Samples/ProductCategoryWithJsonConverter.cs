using Newtonsoft.Json;
using Thinktecture.Json;

namespace Thinktecture
{
   [JsonConverter(typeof(EnumJsonConverter<ProductCategoryWithJsonConverter, string>))]
   public sealed partial class ProductCategoryWithJsonConverter : IEnum<string>
   {
      public static readonly ProductCategoryWithJsonConverter Fruits = new("Fruits");
      public static readonly ProductCategoryWithJsonConverter Dairy = new("Dairy");
   }
}

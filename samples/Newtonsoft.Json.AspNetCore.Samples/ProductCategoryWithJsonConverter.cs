namespace Thinktecture
{
   public sealed partial class ProductCategoryWithJsonConverter : IValidatableEnum<string>
   {
      public static readonly ProductCategoryWithJsonConverter Fruits = new("Fruits");
      public static readonly ProductCategoryWithJsonConverter Dairy = new("Dairy");
   }
}

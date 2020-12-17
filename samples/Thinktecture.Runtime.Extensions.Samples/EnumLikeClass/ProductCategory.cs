namespace Thinktecture.EnumLikeClass
{
   [EnumGeneration(KeyPropertyName = "Name")]
   public sealed partial class ProductCategory : IEnum<string>
   {
      public static readonly ProductCategory Fruits = new("Fruits");
      public static readonly ProductCategory Dairy = new("Dairy");
   }
}

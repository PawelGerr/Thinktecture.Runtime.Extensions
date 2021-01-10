namespace Thinktecture.EnumLikeClasses
{
   [EnumGeneration(KeyPropertyName = "Name")]
   public sealed partial class ProductCategory : IValidatableEnum<string>
   {
      public static readonly ProductCategory Fruits = new("Fruits");
      public static readonly ProductCategory Dairy = new("Dairy");
   }
}

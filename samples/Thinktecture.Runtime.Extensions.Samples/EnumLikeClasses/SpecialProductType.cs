namespace Thinktecture.EnumLikeClasses
{
   [EnumGeneration]
   public sealed partial class SpecialProductType : ProductType
   {
      public static readonly SpecialProductType Special = new("Special");
   }
}

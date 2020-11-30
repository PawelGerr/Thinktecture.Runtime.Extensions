namespace Thinktecture.TestEnums
{
   public partial class EnumWithDuplicateKey : IEnum<string>
   {
      public static readonly EnumWithDuplicateKey Item = new("Item");
      public static readonly EnumWithDuplicateKey Duplicate = new("item");
   }
}

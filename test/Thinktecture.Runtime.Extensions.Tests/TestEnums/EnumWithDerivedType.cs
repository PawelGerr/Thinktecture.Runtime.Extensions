namespace Thinktecture.TestEnums
{
   public partial class EnumWithDerivedType : IValidatableEnum<int>
   {
      public static readonly EnumWithDerivedType Item1 = new(1);
      public static readonly EnumWithDerivedType ItemOfDerivedType = new DerivedEnum(2);

      private class DerivedEnum : EnumWithDerivedType
      {
         public DerivedEnum(int key)
            : base(key)
         {
         }
      }
   }
}

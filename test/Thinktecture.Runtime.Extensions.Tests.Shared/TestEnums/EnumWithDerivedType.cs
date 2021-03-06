namespace Thinktecture.Runtime.Tests.TestEnums
{
   public partial class EnumWithDerivedType : IValidatableEnum<int>
   {
      public static readonly EnumWithDerivedType Item1 = new(1);
      public static readonly EnumWithDerivedType ItemOfDerivedType = new DerivedEnum(2);
      public static readonly EnumWithDerivedType ItemOfInnerType = new DerivedEnum.InnerType(3);

      private class DerivedEnum : EnumWithDerivedType
      {
         public DerivedEnum(int key)
            : base(key)
         {
         }

         public class InnerType : DerivedEnum
         {
            public InnerType(int key)
               : base(key)
            {
            }
         }
      }
   }
}

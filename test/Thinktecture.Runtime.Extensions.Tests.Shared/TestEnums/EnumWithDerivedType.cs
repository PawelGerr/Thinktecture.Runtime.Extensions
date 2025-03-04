namespace Thinktecture.Runtime.Tests.TestEnums;

[SmartEnum<int>(IsValidatable = true)]
public partial class EnumWithDerivedType
{
   public static readonly EnumWithDerivedType Item1 = new(1);
   public static readonly EnumWithDerivedType ItemOfDerivedType = new DerivedEnum(2);
   public static readonly EnumWithDerivedType ItemOfInnerType = new DerivedEnum.InnerType(3);
   public static readonly EnumWithDerivedType GenericItemDecimal = new GenericEnum<decimal>(4);

   private class DerivedEnum : EnumWithDerivedType
   {
      public DerivedEnum(int key)
         : base(key)
      {
      }

      public sealed class InnerType : DerivedEnum
      {
         public InnerType(int key)
            : base(key)
         {
         }
      }
   }

   // ReSharper disable once UnusedTypeParameter
   private sealed class GenericEnum<T> : EnumWithDerivedType
   {
      public GenericEnum(int key)
         : base(key)
      {
      }
   }
}

namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
[SmartEnum<int>]
public partial class SmartEnum_DerivedTypes
{
   public static readonly SmartEnum_DerivedTypes Item1 = new(1);
   public static readonly SmartEnum_DerivedTypes ItemOfDerivedType = new DerivedEnum(2);
   public static readonly SmartEnum_DerivedTypes ItemOfInnerType = new DerivedEnum.InnerType(3);
   public static readonly SmartEnum_DerivedTypes GenericItemDecimal = new GenericEnum<decimal>(4);

   private class DerivedEnum : SmartEnum_DerivedTypes
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
   private sealed class GenericEnum<T> : SmartEnum_DerivedTypes
   {
      public GenericEnum(int key)
         : base(key)
      {
      }
   }
}

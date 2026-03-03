namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
public partial class SmartEnums_NestedInGenericClass
{
   public partial class GenericOuter<T>
   {
      [SmartEnum<int>]
      public sealed partial class KeyedSmartEnum
      {
         public static readonly KeyedSmartEnum Item = new(1);
      }

      [SmartEnum]
      public sealed partial class KeylessSmartEnum
      {
         public static readonly KeylessSmartEnum Item = new();
      }
   }
}

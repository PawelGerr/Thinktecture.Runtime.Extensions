namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
[SmartEnum<string>]
public partial class SmartEnum_LendingOtherSmartEnumItems
{
   public static readonly SmartEnum_LendingOtherSmartEnumItems Item1 = new("item1");
   public static readonly SmartEnum_LendingOtherSmartEnumItems Item2 = new("item2");

   public static readonly SmartEnum_IntBased Item6 = SmartEnum_IntBased.Item1;
   public static readonly SmartEnum_IntBased Item7 = SmartEnum_IntBased.Get(2);
}

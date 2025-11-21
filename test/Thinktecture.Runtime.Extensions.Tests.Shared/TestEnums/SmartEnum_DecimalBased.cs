namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable InconsistentNaming
[SmartEnum<decimal>]
public partial class SmartEnum_DecimalBased
{
   public static readonly SmartEnum_DecimalBased Value1 = new(1);
   public static readonly SmartEnum_DecimalBased Value1_5 = new(1.5m);
   public static readonly SmartEnum_DecimalBased Value2 = new(2);
   public static readonly SmartEnum_DecimalBased Value3 = new(3);
   public static readonly SmartEnum_DecimalBased Value4 = new(4);
   public static readonly SmartEnum_DecimalBased Value5 = new(5);
}

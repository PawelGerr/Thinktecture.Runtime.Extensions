namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable InconsistentNaming
[SmartEnum<int>(IsValidatable = true)]
public partial class TestSmartEnum_Class_IntBased_Validatable
{
   public static readonly TestSmartEnum_Class_IntBased_Validatable Value1 = new(1);
   public static readonly TestSmartEnum_Class_IntBased_Validatable Value2 = new(2);
   public static readonly TestSmartEnum_Class_IntBased_Validatable Value3 = new(3);
   public static readonly TestSmartEnum_Class_IntBased_Validatable Value4 = new(4);
   public static readonly TestSmartEnum_Class_IntBased_Validatable Value5 = new(5);
}

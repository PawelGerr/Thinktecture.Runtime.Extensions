namespace Thinktecture.Database;

// ReSharper disable InconsistentNaming
[SmartEnum<int>(IsValidatable = true)]
public readonly partial struct TestSmartEnum_Struct_IntBased
{
   public static readonly TestSmartEnum_Struct_IntBased Value1 = new(0);
   public static readonly TestSmartEnum_Struct_IntBased Value2 = new(1);
   public static readonly TestSmartEnum_Struct_IntBased Value3 = new(2);
   public static readonly TestSmartEnum_Struct_IntBased Value4 = new(3);
   public static readonly TestSmartEnum_Struct_IntBased Value5 = new(4);
}

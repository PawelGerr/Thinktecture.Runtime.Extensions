namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
[SmartEnum<int>(IsValidatable = true,
                SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
                MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
public partial struct TestSmartEnum_Struct_IntBased
{
   public static readonly TestSmartEnum_Struct_IntBased Value1 = new(1);
   public static readonly TestSmartEnum_Struct_IntBased Value2 = new(2);
   public static readonly TestSmartEnum_Struct_IntBased Value3 = new(3);
   public static readonly TestSmartEnum_Struct_IntBased Value4 = new(4);
   public static readonly TestSmartEnum_Struct_IntBased Value5 = new(5);
}

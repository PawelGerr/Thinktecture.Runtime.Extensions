namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
[SmartEnum<int>(IsValidatable = true,
                SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
                MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
                ComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                EqualityComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads)]
public partial struct TestSmartEnum_Struct_IntBased_Validatable
{
   public static readonly TestSmartEnum_Struct_IntBased_Validatable Value1 = new(1);
   public static readonly TestSmartEnum_Struct_IntBased_Validatable Value2 = new(2);
   public static readonly TestSmartEnum_Struct_IntBased_Validatable Value3 = new(3);
   public static readonly TestSmartEnum_Struct_IntBased_Validatable Value4 = new(4);
   public static readonly TestSmartEnum_Struct_IntBased_Validatable Value5 = new(5);
}


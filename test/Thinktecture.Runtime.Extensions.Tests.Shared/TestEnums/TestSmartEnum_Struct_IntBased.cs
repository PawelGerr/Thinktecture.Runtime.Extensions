namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
public readonly partial struct TestSmartEnum_Struct_IntBased : IValidatableEnum<int>
{
   public static readonly TestSmartEnum_Struct_IntBased Value1 = new(1);
   public static readonly TestSmartEnum_Struct_IntBased Value2 = new(2);
   public static readonly TestSmartEnum_Struct_IntBased Value3 = new(3);
   public static readonly TestSmartEnum_Struct_IntBased Value4 = new(4);
   public static readonly TestSmartEnum_Struct_IntBased Value5 = new(5);
}

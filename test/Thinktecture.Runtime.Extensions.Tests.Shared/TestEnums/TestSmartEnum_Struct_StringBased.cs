namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
[SmartEnum<string>(IsValidatable = true)]
public partial struct TestSmartEnum_Struct_StringBased
{
   public static readonly TestSmartEnum_Struct_StringBased Value1 = new("Value1");
   public static readonly TestSmartEnum_Struct_StringBased Value2 = new("Value2");
   public static readonly TestSmartEnum_Struct_StringBased Value3 = new("Value3");
   public static readonly TestSmartEnum_Struct_StringBased Value4 = new("Value4");
   public static readonly TestSmartEnum_Struct_StringBased Value5 = new("Value5");
}

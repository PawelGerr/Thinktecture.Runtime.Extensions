namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
[SmartEnum<string>(IsValidatable = true)]
public partial class TestSmartEnum_Class_StringBased_Validatable
{
   public static readonly TestSmartEnum_Class_StringBased_Validatable Value1 = new("Value1");
   public static readonly TestSmartEnum_Class_StringBased_Validatable Value2 = new("Value2");
   public static readonly TestSmartEnum_Class_StringBased_Validatable Value3 = new("Value3");
   public static readonly TestSmartEnum_Class_StringBased_Validatable Value4 = new("Value4");
   public static readonly TestSmartEnum_Class_StringBased_Validatable Value5 = new("Value5");
}

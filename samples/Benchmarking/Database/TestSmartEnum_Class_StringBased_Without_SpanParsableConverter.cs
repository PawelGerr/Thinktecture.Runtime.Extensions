namespace Thinktecture.Database;

// ReSharper disable InconsistentNaming
[SmartEnum<string>(DisableSpanBasedJsonConversion = true)]
public partial class TestSmartEnum_Class_StringBased_Without_SpanParsableConverter
{
   public static readonly TestSmartEnum_Class_StringBased_Without_SpanParsableConverter Value1 = new("Value1");
   public static readonly TestSmartEnum_Class_StringBased_Without_SpanParsableConverter Value2 = new("Value2");
   public static readonly TestSmartEnum_Class_StringBased_Without_SpanParsableConverter Value3 = new("Value3");
   public static readonly TestSmartEnum_Class_StringBased_Without_SpanParsableConverter Value4 = new("Value4");
   public static readonly TestSmartEnum_Class_StringBased_Without_SpanParsableConverter Value5 = new("Value5");
}

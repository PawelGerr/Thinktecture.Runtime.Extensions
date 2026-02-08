namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
[SmartEnum<string>(DisableSpanBasedJsonConversion = true)]
public partial class SmartEnum_StringBased_WithDisabledSpanBasedJsonConversion
{
   public static readonly SmartEnum_StringBased_WithDisabledSpanBasedJsonConversion Item1 = new("Item1");
   public static readonly SmartEnum_StringBased_WithDisabledSpanBasedJsonConversion Item2 = new("Item2");
}

namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
[SmartEnum<CustomSpanParsableKey>]
public partial class SmartEnum_CustomSpanParsableKey
{
   public static readonly SmartEnum_CustomSpanParsableKey Item1 = new(new CustomSpanParsableKey(1));
   public static readonly SmartEnum_CustomSpanParsableKey Item2 = new(new CustomSpanParsableKey(2));
}

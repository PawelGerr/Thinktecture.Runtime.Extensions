namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
[SmartEnum<long>]
public partial class SmartEnum_LongBased
{
   public static readonly SmartEnum_LongBased Item1 = new(1L);
   public static readonly SmartEnum_LongBased Item2 = new(2L);
}

namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
[SmartEnum<int>(SkipIFormattable = true)]
public partial class SmartEnum_without_IFormattable
{
   public static readonly SmartEnum_without_IFormattable Item1 = new(1);
   public static readonly SmartEnum_without_IFormattable Item2 = new(2);
}

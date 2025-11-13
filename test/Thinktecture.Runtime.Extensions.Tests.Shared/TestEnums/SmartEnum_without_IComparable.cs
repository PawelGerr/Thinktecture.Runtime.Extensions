namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
[SmartEnum<int>(SkipIComparable = true)]
public partial class SmartEnum_without_IComparable
{
   public static readonly SmartEnum_without_IComparable Item1 = new(1);
   public static readonly SmartEnum_without_IComparable Item2 = new(2);
}

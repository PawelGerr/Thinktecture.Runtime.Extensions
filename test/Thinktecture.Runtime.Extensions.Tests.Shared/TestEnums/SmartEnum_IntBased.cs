namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
[SmartEnum<int>]
public partial class SmartEnum_IntBased
{
   public static readonly SmartEnum_IntBased Item1 = new(1);
   public static readonly SmartEnum_IntBased Item2 = new(2);
   public static readonly SmartEnum_IntBased Item3 = new(3);
   public static readonly SmartEnum_IntBased Item4 = new(4);
   public static readonly SmartEnum_IntBased Item5 = new(5);
}

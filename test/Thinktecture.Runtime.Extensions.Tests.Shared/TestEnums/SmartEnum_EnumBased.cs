namespace Thinktecture.Runtime.Tests.TestEnums;

public enum SmartEnum_EnumKey
{
   Item1 = 1,
   Item2 = 2,
   Item3 = 3,
}

// ReSharper disable once InconsistentNaming
[SmartEnum<SmartEnum_EnumKey>]
public partial class SmartEnum_EnumBased
{
   public static readonly SmartEnum_EnumBased Item1 = new(SmartEnum_EnumKey.Item1);
   public static readonly SmartEnum_EnumBased Item2 = new(SmartEnum_EnumKey.Item2);
   public static readonly SmartEnum_EnumBased Item3 = new(SmartEnum_EnumKey.Item3);
}

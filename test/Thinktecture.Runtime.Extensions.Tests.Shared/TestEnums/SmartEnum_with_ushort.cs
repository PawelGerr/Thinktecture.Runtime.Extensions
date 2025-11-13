namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
[SmartEnum<ushort>]
public partial class SmartEnum_with_ushort
{
   public static readonly SmartEnum_with_ushort Item1 = new(1);
   public static readonly SmartEnum_with_ushort Item2 = new(2);
}

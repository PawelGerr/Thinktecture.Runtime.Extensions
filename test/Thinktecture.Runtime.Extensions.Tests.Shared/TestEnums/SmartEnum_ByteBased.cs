namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
[SmartEnum<byte>]
public partial class SmartEnum_ByteBased
{
   public static readonly SmartEnum_ByteBased Item1 = new(1);
   public static readonly SmartEnum_ByteBased Item2 = new(2);
}

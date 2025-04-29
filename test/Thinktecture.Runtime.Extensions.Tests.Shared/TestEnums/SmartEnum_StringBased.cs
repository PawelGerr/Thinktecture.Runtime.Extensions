namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
[SmartEnum<string>]
public partial class SmartEnum_StringBased
{
   public static readonly SmartEnum_StringBased Item1 = new("Item1");
   public static readonly SmartEnum_StringBased Item2 = new("Item2");
}

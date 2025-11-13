namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
[SmartEnum<float>]
public partial class SmartEnum_with_float
{
   public static readonly SmartEnum_with_float Item1 = new(1.5f);
   public static readonly SmartEnum_with_float Item2 = new(2.5f);
}

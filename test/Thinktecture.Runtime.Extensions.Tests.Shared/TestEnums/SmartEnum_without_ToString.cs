namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
[SmartEnum<string>(SkipToString = true)]
public partial class SmartEnum_without_ToString
{
   public static readonly SmartEnum_without_ToString Item1 = new("Item1");
   public static readonly SmartEnum_without_ToString Item2 = new("Item2");
}

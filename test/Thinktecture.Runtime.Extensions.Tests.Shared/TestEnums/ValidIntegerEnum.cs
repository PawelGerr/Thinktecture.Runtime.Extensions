namespace Thinktecture.Runtime.Tests.TestEnums;

[SmartEnum<int>]
public sealed partial class ValidIntegerEnum
{
   public static readonly ValidIntegerEnum Item1 = new(1);
   public static readonly ValidIntegerEnum Item2 = new(2);
}

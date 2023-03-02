namespace Thinktecture.Runtime.Tests.TestEnums;

public sealed partial class ValidIntegerEnum : IEnum<int>
{
   public static readonly ValidIntegerEnum Item1 = new(1);
   public static readonly ValidIntegerEnum Item2 = new(2);
}

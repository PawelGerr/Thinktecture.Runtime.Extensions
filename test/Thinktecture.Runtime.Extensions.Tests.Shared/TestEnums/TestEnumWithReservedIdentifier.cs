namespace Thinktecture.Runtime.Tests.TestEnums;

public sealed partial class TestEnumWithReservedIdentifier : IEnum<int>
{
   public static readonly TestEnumWithReservedIdentifier Operator = new(1);
   public static readonly TestEnumWithReservedIdentifier True = new(2);
}

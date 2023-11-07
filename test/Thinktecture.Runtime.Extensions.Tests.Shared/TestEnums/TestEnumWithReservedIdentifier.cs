namespace Thinktecture.Runtime.Tests.TestEnums;

[SmartEnum<int>]
public sealed partial class TestEnumWithReservedIdentifier
{
   public static readonly TestEnumWithReservedIdentifier Operator = new(1);
   public static readonly TestEnumWithReservedIdentifier True = new(2);
}

namespace Thinktecture.Runtime.Tests.TestEnums;

[SmartEnum<string>]
[ValueObjectValidationError<TestEnumValidationError>]
public sealed partial class TestEnumWithCustomError
{
   public static readonly TestEnumWithCustomError Item1 = new("item1");
   public static readonly TestEnumWithCustomError Item2 = new("item2");
}

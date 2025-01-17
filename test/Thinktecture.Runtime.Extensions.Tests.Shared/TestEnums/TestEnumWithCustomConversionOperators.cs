namespace Thinktecture.Runtime.Tests.TestEnums;

[SmartEnum<int>(ConversionToKeyMemberType = ConversionOperatorsGeneration.Explicit,
                ConversionFromKeyMemberType = ConversionOperatorsGeneration.Implicit)]
public partial class TestEnumWithCustomConversionOperators
{
   public static readonly TestEnumWithCustomConversionOperators Item = new(1);
}

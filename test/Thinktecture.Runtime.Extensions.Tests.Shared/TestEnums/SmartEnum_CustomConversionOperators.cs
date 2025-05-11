namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
[SmartEnum<int>(ConversionToKeyMemberType = ConversionOperatorsGeneration.Explicit,
                ConversionFromKeyMemberType = ConversionOperatorsGeneration.Implicit)]
public partial class SmartEnum_CustomConversionOperators
{
   public static readonly SmartEnum_CustomConversionOperators Item = new(1);
}

namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ValueObject<int>(ConversionToKeyMemberType = ConversionOperatorsGeneration.Explicit,
                  UnsafeConversionToKeyMemberType = ConversionOperatorsGeneration.Implicit,
                  ConversionFromKeyMemberType = ConversionOperatorsGeneration.Implicit)]
public partial class TestValueObjectWithCustomConversionOperators;

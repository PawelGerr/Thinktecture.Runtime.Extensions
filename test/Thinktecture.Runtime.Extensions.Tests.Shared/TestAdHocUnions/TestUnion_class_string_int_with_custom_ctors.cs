namespace Thinktecture.Runtime.Tests.TestAdHocUnions;

// ReSharper disable once InconsistentNaming
[Union<string, int>(ConversionFromValue = ConversionOperatorsGeneration.None,
                    ConstructorAccessModifier = UnionConstructorAccessModifier.Private)]
public partial class TestUnion_class_string_int_with_custom_ctors
{
   public required object OtherValue { get; init; }

   public TestUnion_class_string_int_with_custom_ctors(string value, object otherValue)
      : this(value)
   {
      OtherValue = otherValue;
   }

   public TestUnion_class_string_int_with_custom_ctors(int value, object otherValue)
      : this(value)
   {
      OtherValue = otherValue;
   }
}

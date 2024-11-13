namespace Thinktecture.Runtime.Tests.TestAdHocUnions;

// ReSharper disable once InconsistentNaming
[Union<string, int>(SkipImplicitConversionFromValue = true,
                    ConstructorAccessModifier = UnionConstructorAccessModifier.Private)]
public partial class TestUnion_class_string_int_with_private_ctors
{
   public required object OtherValue { get; init; }

   public TestUnion_class_string_int_with_private_ctors(string value, object otherValue)
      : this(value)
   {
      OtherValue = otherValue;
   }

   public TestUnion_class_string_int_with_private_ctors(int value, object otherValue)
      : this(value)
   {
      OtherValue = otherValue;
   }
}

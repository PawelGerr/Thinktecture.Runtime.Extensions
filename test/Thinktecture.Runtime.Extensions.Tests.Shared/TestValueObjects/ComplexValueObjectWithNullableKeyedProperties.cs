namespace Thinktecture.Runtime.Tests.TestValueObjects;

// ReSharper disable once InconsistentNaming
[ComplexValueObject]
public partial class ComplexValueObjectWithNullableKeyedProperties
{
   public StringBasedReferenceValueObject? NullableStringBasedValueObject { get; }
   public IntBasedReferenceValueObject? NullableIntBasedValueObject { get; }
   public IntBasedStructValueObject? NullableIntBasedStructValueObject { get; }
   public int OtherProperty { get; }
}

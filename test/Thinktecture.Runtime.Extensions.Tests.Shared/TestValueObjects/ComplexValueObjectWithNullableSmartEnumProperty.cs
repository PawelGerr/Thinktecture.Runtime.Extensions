using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.TestValueObjects;

// ReSharper disable once InconsistentNaming
[ComplexValueObject]
public partial class ComplexValueObjectWithNullableSmartEnumProperty
{
   public SmartEnum_StringBased? NullableStringBasedSmartEnum { get; }
   public SmartEnum_IntBased? NullableIntBasedSmartEnum { get; }
   public int OtherProperty { get; }
}

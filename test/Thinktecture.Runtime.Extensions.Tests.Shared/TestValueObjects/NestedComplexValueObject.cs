namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ComplexValueObject]
public partial class NestedComplexValueObject
{
   public ChildComplexValueObject ChildComplexValueObject { get; }
   public IntBasedStructValueObject IntBasedStructValueObject { get; }
   public StringBasedReferenceValueObject StringBasedReferenceValueObject { get; }
}

[ComplexValueObject]
public partial class ChildComplexValueObject
{
   public IntBasedStructValueObject IntBasedStructValueObject { get; }
   public StringBasedReferenceValueObject StringBasedReferenceValueObject { get; }
}

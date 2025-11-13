namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ComplexValueObject(AllowDefaultStructs = false)]
public partial struct ComplexValueObjectWithAllowDefaultStructsSetToFalse
{
   public int Property { get; }
}

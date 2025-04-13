namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ComplexValueObject]
public partial struct ComplexValueObjectDoesNotAllowDefaultStructsWithIntBasedStruct
{
   public IntBasedStructValueObjectDoesNotAllowDefaultStructs Property { get; }
}

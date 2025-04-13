namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ComplexValueObject]
public partial struct ComplexValueObjectDoesNotAllowDefaultStructsWithStringBasedStruct
{
   public StringBasedStructValueObject Property { get; }
}

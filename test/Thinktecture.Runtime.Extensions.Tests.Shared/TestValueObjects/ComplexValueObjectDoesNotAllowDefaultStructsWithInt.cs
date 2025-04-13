namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ComplexValueObject]
public partial struct ComplexValueObjectDoesNotAllowDefaultStructsWithInt
{
   public int Property { get; }
}

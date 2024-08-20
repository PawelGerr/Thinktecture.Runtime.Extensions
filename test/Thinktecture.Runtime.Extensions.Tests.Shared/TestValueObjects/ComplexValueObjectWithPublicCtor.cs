namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ComplexValueObject(ConstructorAccessModifier = ValueObjectAccessModifier.Public)]
public partial class ComplexValueObjectWithPublicCtor
{
   public decimal Lower { get; }
   public decimal Upper { get; }
}

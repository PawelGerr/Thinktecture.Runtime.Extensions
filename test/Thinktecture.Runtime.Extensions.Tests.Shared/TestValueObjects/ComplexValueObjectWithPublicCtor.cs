namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ComplexValueObject(ConstructorAccessModifier = ValueObjectAccessModifier.Public)]
public sealed partial class ComplexValueObjectWithPublicCtor
{
   public decimal Lower { get; }
   public decimal Upper { get; }
}

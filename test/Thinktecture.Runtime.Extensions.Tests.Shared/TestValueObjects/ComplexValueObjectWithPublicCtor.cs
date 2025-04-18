namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ComplexValueObject(ConstructorAccessModifier = AccessModifier.Public)]
public partial class ComplexValueObjectWithPublicCtor
{
   public decimal Lower { get; }
   public decimal Upper { get; }
}

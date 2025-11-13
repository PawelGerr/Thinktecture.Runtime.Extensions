namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ComplexValueObject(SkipToString = true)]
public partial class ComplexValueObjectWithoutToString
{
   public int Property { get; }
}

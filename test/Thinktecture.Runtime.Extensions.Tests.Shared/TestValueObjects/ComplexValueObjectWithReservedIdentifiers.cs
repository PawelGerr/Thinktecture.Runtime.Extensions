namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ValueObject]
public sealed partial class ComplexValueObjectWithReservedIdentifiers
{
   public int Operator { get; }
   public int? True { get; }
}

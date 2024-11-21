namespace Thinktecture.Runtime.Tests.TestValueObjects;

// ReSharper disable once InconsistentNaming
[ComplexValueObject(AllowDefaultStructs = true)]
public partial struct TestValueObject_Complex_Struct
{
   public string Property1 { get; }
   public string Property2 { get; }
}

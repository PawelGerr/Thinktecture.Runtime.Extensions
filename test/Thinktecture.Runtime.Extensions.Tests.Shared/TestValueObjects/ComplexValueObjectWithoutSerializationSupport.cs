namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ComplexValueObject(SerializationFrameworks = SerializationFrameworks.None)]
public partial class ComplexValueObjectWithoutSerializationSupport
{
   public int Property { get; }
}

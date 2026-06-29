namespace Thinktecture.Runtime.Tests.TestValueObjects;

public enum ValueObject_EnumKey
{
   Item1 = 1,
   Item2 = 2,
   Item3 = 3,
}

[ValueObject<ValueObject_EnumKey>]
public partial class EnumBasedValueObject;

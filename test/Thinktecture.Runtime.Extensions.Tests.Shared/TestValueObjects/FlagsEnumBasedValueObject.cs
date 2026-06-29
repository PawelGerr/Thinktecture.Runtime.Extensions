using System;

namespace Thinktecture.Runtime.Tests.TestValueObjects;

[Flags]
public enum ValueObject_FlagsEnumKey
{
   None = 0,
   First = 1,
   Second = 2,
   Third = 4,
}

[ValueObject<ValueObject_FlagsEnumKey>]
public partial class FlagsEnumBasedValueObject;

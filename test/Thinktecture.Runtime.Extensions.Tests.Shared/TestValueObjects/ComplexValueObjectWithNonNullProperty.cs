using System;

namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ComplexValueObject(DefaultStringComparison = StringComparison.OrdinalIgnoreCase)]
public partial struct ComplexValueObjectWithNonNullProperty
{
   public string Property { get; }
}

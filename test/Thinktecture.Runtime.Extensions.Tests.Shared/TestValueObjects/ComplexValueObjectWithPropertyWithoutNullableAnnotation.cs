using System;

namespace Thinktecture.Runtime.Tests.TestValueObjects;

#nullable disable

[ComplexValueObject(DefaultStringComparison = StringComparison.OrdinalIgnoreCase)]
public partial struct ComplexValueObjectWithPropertyWithoutNullableAnnotation
{
   public string Property { get; }
}

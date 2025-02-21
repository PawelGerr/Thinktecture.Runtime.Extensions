using System;

namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ComplexValueObject(DefaultStringComparison = StringComparison.Ordinal)]
public partial class ComplexValueObjectWithString
{
   public string Property { get; }
}

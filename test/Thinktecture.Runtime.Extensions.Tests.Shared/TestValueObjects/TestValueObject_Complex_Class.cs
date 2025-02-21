using System;

namespace Thinktecture.Runtime.Tests.TestValueObjects;

// ReSharper disable once InconsistentNaming
[ComplexValueObject(DefaultStringComparison = StringComparison.OrdinalIgnoreCase)]
public partial class TestValueObject_Complex_Class
{
   public string Property1 { get; }
   public string Property2 { get; }
}

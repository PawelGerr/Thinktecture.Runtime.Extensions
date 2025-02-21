using System;

namespace Thinktecture.Runtime.Tests.Formatters.EnumMessagePackFormatterTests.TestClasses;

// ReSharper disable InconsistentNaming
[ComplexValueObject(
   AllowDefaultStructs = true,
   DefaultStringComparison = StringComparison.OrdinalIgnoreCase)]
public partial struct TestValueObject_Complex_Struct_WithFormatter
{
   public string Property1 { get; }
   public string Property2 { get; }
}

using System;

namespace Thinktecture.Runtime.Tests.Formatters.ThinktectureMessagePackFormatterTests.TestClasses;

// ReSharper disable InconsistentNaming
[ComplexValueObject(
   AllowDefaultStructs = true,
   DefaultStringComparison = StringComparison.OrdinalIgnoreCase)]
public partial struct TestValueObject_Complex_Struct_WithFormatter
{
   public string Property1 { get; }
   public string Property2 { get; }
}

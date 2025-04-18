using System;

namespace Thinktecture.Runtime.Tests.Formatters.ThinktectureMessagePackFormatterTests.TestClasses;

// ReSharper disable InconsistentNaming
[ComplexValueObject(DefaultStringComparison = StringComparison.OrdinalIgnoreCase)]
public partial class TestValueObject_Complex_Class_WithFormatter
{
   public string Property1 { get; }
   public string Property2 { get; }
}

using System;

namespace Thinktecture.Runtime.Tests.Json.ThinktectureNewtonsoftJsonConverterTests.TestClasses;

[ComplexValueObject(DefaultStringComparison = StringComparison.OrdinalIgnoreCase)]
public partial class ValueObjectWithMultipleProperties
{
   public decimal StructProperty { get; }
   public int? NullableStructProperty { get; }
   public string ReferenceProperty { get; }
}

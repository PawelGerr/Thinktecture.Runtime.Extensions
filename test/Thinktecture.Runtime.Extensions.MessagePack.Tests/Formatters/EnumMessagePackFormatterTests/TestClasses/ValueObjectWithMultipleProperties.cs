namespace Thinktecture.Runtime.Tests.Formatters.EnumMessagePackFormatterTests.TestClasses;

[ComplexValueObject]
public sealed partial class ValueObjectWithMultipleProperties
{
   public decimal StructProperty { get; }
   public int? NullableStructProperty { get; }
   public string ReferenceProperty { get; }
}

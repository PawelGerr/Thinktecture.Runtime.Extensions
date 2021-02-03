namespace Thinktecture.Runtime.Tests.Text.Json.Serialization.ValueTypeJsonConverterFactoryTests.TestClasses
{
   [ValueType]
   public partial class ValueTypeWithMultipleProperties
   {
      public decimal StructProperty { get; }
      public int? NullableStructProperty { get; }
      public string ReferenceProperty { get; }
   }
}

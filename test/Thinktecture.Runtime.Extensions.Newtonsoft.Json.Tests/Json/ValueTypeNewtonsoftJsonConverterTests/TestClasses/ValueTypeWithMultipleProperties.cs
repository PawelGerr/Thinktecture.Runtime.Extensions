namespace Thinktecture.Runtime.Tests.Json.ValueTypeNewtonsoftJsonConverterTests.TestClasses
{
   [ValueType]
   public partial class ValueTypeWithMultipleProperties
   {
      public decimal StructProperty { get; }
      public int? NullableStructProperty { get; }
      public string ReferenceProperty { get; }
   }
}

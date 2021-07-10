namespace Thinktecture.Runtime.Tests.Json.ValueObjectNewtonsoftJsonConverterTests.TestClasses
{
   [ValueObject]
   public partial class ValueObjectWithMultipleProperties
   {
      public decimal StructProperty { get; }
      public int? NullableStructProperty { get; }
      public string ReferenceProperty { get; }
   }
}

namespace Thinktecture.Runtime.Tests.Text.Json.Serialization.ValueObjectJsonConverterFactoryTests.TestClasses
{
   [ValueObject]
   public partial class ValueObjectWithMultipleProperties
   {
      public decimal StructProperty { get; }
      public int? NullableStructProperty { get; }
      public string ReferenceProperty { get; }
   }
}

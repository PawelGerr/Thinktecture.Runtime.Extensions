namespace Thinktecture.Runtime.Tests.Formatters.EnumMessagePackFormatterTests.TestClasses
{
   [ValueType]
   public partial class ValueTypeWithMultipleProperties
   {
      public decimal StructProperty { get; }
      public int? NullableStructProperty { get; }
      public string ReferenceProperty { get; }
   }
}

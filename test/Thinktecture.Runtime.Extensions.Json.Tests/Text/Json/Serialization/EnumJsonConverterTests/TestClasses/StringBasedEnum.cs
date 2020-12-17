namespace Thinktecture.Text.Json.Serialization.EnumJsonConverterTests.TestClasses
{
   public partial class StringBasedEnum : IEnum<string>
   {
      public static readonly StringBasedEnum ValueA = new("A");
      public static readonly StringBasedEnum ValueB = new("B");
   }
}

namespace Thinktecture.Json.EnumJsonConverterTests
{
   public partial class StringBasedEnum : IEnum<string>
   {
      public static readonly StringBasedEnum ValueA = new("A");
      public static readonly StringBasedEnum ValueB = new("B");

      IEnum<string> IEnum<string>.CreateInvalid(string key)
      {
         return new StringBasedEnum(key);
      }
   }
}

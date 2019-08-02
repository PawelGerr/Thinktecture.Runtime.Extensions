namespace Thinktecture.Json.EnumJsonConverterTests
{
   public class StringBasedEnum : Enum<StringBasedEnum>
   {
      public static readonly StringBasedEnum ValueA = new StringBasedEnum("A");
      public static readonly StringBasedEnum ValueB = new StringBasedEnum("B");

      private StringBasedEnum(string key)
         : base(key)
      {
      }

      protected override StringBasedEnum CreateInvalid(string key)
      {
         return new StringBasedEnum(key);
      }
   }
}

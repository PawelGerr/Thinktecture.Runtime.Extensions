namespace Thinktecture.Text.Json.Serialization.EnumJsonConverterTests.TestClasses
{
   public class StringBasedEnum : Enum<StringBasedEnum>
   {
      public static readonly StringBasedEnum ValueA = new("A");
      public static readonly StringBasedEnum ValueB = new("B");

      private StringBasedEnum(string key)
         : base(key)
      {
      }

      protected override StringBasedEnum CreateInvalid(string key)
      {
         return new(key);
      }
   }
}

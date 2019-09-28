namespace Thinktecture.Text.Json.Serialization.EnumJsonConverterTests.TestClasses
{
   public class ClassWithStringBasedEnum
   {
      public StringBasedEnum Enum { get; set; }

      public ClassWithStringBasedEnum()
      {
      }

      public ClassWithStringBasedEnum(StringBasedEnum value)
      {
         Enum = value;
      }
   }
}

namespace Thinktecture.Text.Json.Serialization.EnumJsonConverterTests.TestClasses
{
   public class ClassWithIntBasedEnum
   {
      public IntBasedEnum Enum { get; set; }

      public ClassWithIntBasedEnum()
      {
      }

      public ClassWithIntBasedEnum(IntBasedEnum value)
      {
         Enum = value;
      }
   }
}

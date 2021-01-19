namespace Thinktecture.Text.Json.Serialization.ValueTypeJsonConverterFactoryTests.TestClasses
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

using MessagePack;

namespace Thinktecture.Runtime.Tests.Formatters.EnumMessagePackFormatterTests.TestClasses
{
   [MessagePackObject]
   public class ClassWithStringBasedEnum
   {
      [Key(0)]
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

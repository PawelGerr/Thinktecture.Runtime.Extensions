using MessagePack;

namespace Thinktecture.Runtime.Tests.Formatters.EnumMessagePackFormatterTests.TestClasses
{
   [MessagePackObject]
   public class ClassWithIntBasedEnum
   {
      [Key(0)]
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

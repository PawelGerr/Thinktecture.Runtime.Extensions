using MessagePack;
using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.Formatters.EnumMessagePackFormatterTests.TestClasses
{
   [MessagePackObject]
   public class ClassWithIntBasedEnum
   {
      [Key(0)]
      public IntegerEnum Enum { get; set; }

      public ClassWithIntBasedEnum()
      {
      }

      public ClassWithIntBasedEnum(IntegerEnum value)
      {
         Enum = value;
      }
   }
}

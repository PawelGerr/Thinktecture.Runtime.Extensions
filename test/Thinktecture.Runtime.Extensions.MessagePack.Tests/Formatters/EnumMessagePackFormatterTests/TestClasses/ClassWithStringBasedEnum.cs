using MessagePack;
using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.Formatters.EnumMessagePackFormatterTests.TestClasses
{
   [MessagePackObject]
   public class ClassWithStringBasedEnum
   {
      [Key(0)]
      public TestEnum Enum { get; set; }

      public ClassWithStringBasedEnum()
      {
      }

      public ClassWithStringBasedEnum(TestEnum value)
      {
         Enum = value;
      }
   }
}

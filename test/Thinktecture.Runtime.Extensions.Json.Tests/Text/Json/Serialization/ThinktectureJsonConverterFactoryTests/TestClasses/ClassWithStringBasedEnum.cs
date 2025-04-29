using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.Text.Json.Serialization.ThinktectureJsonConverterFactoryTests.TestClasses;

public class ClassWithStringBasedEnum
{
   public SmartEnum_StringBased Enum { get; set; }

   public ClassWithStringBasedEnum()
   {
   }

   public ClassWithStringBasedEnum(SmartEnum_StringBased value)
   {
      Enum = value;
   }
}

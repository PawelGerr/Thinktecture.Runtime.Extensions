using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.Text.Json.Serialization.ThinktectureJsonConverterFactoryTests.TestClasses;

public class ClassWithIntBasedEnum
{
   public SmartEnum_IntBased Enum { get; set; }

   public ClassWithIntBasedEnum()
   {
   }

   public ClassWithIntBasedEnum(SmartEnum_IntBased value)
   {
      Enum = value;
   }
}

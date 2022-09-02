using System.Collections.Generic;
using Thinktecture.Runtime.Tests.TestEnums;
using Thinktecture.Runtime.Tests.Text.Json.Serialization.ValueObjectJsonConverterFactoryTests.TestClasses;

namespace Thinktecture.Runtime.Tests.Text.Json.Serialization.ValueObjectJsonConverterFactoryTests;

public class JsonTestsBase
{
   public static IEnumerable<object[]> DataForStringBasedEnumTest => new[]
                                                                     {
                                                                        new object[] { null, "null" },
                                                                        new object[] { TestEnum.Item1, "\"item1\"" },
                                                                        new object[] { TestEnum.Item2, "\"item2\"" }
                                                                     };

   public static IEnumerable<object[]> DataForClassWithStringBasedEnumTest => new[]
                                                                              {
                                                                                 new object[] { null, "null" },
                                                                                 new object[] { new ClassWithStringBasedEnum(), "{}", true },
                                                                                 new object[] { new ClassWithStringBasedEnum(), "{\"Enum\":null}" },
                                                                                 new object[] { new ClassWithStringBasedEnum(TestEnum.Item1), "{\"Enum\":\"item1\"}" },
                                                                                 new object[] { new ClassWithStringBasedEnum(TestEnum.Item2), "{\"Enum\":\"item2\"}" }
                                                                              };

   public static IEnumerable<object[]> DataForIntBasedEnumTest => new[]
                                                                  {
                                                                     new object[] { null, "null" },
                                                                     new object[] { IntegerEnum.Item1, "1" },
                                                                     new object[] { IntegerEnum.Item2, "2" }
                                                                  };

   public static IEnumerable<object[]> DataForClassWithIntBasedEnumTest => new[]
                                                                           {
                                                                              new object[] { null, "null" },
                                                                              new object[] { new ClassWithIntBasedEnum(), "{}", true },
                                                                              new object[] { new ClassWithIntBasedEnum(), "{\"Enum\":null}" },
                                                                              new object[] { new ClassWithIntBasedEnum(IntegerEnum.Item1), "{\"Enum\":1}" },
                                                                              new object[] { new ClassWithIntBasedEnum(IntegerEnum.Item2), "{\"Enum\":2}" }
                                                                           };
}

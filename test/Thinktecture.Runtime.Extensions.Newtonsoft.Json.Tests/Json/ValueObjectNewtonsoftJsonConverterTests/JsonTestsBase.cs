using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.Json.ValueObjectNewtonsoftJsonConverterTests;

public class JsonTestsBase
{
   public static IEnumerable<object[]> DataForStringBasedEnumTest => new[]
                                                                     {
                                                                        new object[] { null, "null" },
                                                                        new object[] { TestEnum.Item1, "\"item1\"" },
                                                                        new object[] { TestEnum.Item2, "\"item2\"" }
                                                                     };

   public static IEnumerable<object[]> DataForExtensibleTestEnumTest => new[]
                                                                        {
                                                                           new object[] { null, "null" },
                                                                           new object[] { ExtensibleTestEnum.Item1, "\"Item1\"" },
                                                                        };

   public static IEnumerable<object[]> DataForExtendedTestEnumTest => new[]
                                                                      {
                                                                         new object[] { null, "null" },
                                                                         new object[] { ExtendedTestEnum.Item1, "\"Item1\"" },
                                                                         new object[] { ExtendedTestEnum.Item2, "\"Item2\"" }
                                                                      };

   public static IEnumerable<object[]> DataForDifferentAssemblyExtendedTestEnumTest => new[]
                                                                                       {
                                                                                          new object[] { null, "null" },
                                                                                          new object[] { DifferentAssemblyExtendedTestEnum.Item1, "\"Item1\"" },
                                                                                          new object[] { DifferentAssemblyExtendedTestEnum.Item2, "\"Item2\"" }
                                                                                       };

   public static IEnumerable<object[]> DataForIntBasedEnumTest => new[]
                                                                  {
                                                                     new object[] { null, "null" },
                                                                     new object[] { IntegerEnum.Item1, "1" },
                                                                     new object[] { IntegerEnum.Item2, "2" }
                                                                  };
}
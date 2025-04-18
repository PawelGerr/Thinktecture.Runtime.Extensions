using System.Collections.Generic;
using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.Json.ThinktectureNewtonsoftJsonConverterTests;

public class JsonTestsBase
{
   public static IEnumerable<object[]> DataForStringBasedEnumTest =>
   [
      [null, "null"],
      [TestEnum.Item1, "\"item1\""],
      [TestEnum.Item2, "\"item2\""]
   ];

   public static IEnumerable<object[]> DataForIntBasedEnumTest =>
   [
      [null, "null"],
      [IntegerEnum.Item1, "1"],
      [IntegerEnum.Item2, "2"]
   ];
}

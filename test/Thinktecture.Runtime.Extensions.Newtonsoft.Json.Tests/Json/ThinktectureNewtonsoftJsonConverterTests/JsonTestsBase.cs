using System.Collections.Generic;
using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.Json.ThinktectureNewtonsoftJsonConverterTests;

public class JsonTestsBase
{
   public static IEnumerable<object[]> DataForStringBasedEnumTest =>
   [
      [null, "null"],
      [SmartEnum_StringBased.Item1, "\"Item1\""],
      [SmartEnum_StringBased.Item2, "\"Item2\""]
   ];

   public static IEnumerable<object[]> DataForIntBasedEnumTest =>
   [
      [null, "null"],
      [SmartEnum_IntBased.Item1, "1"],
      [SmartEnum_IntBased.Item2, "2"]
   ];
}

using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Thinktecture.Runtime.Tests.TestEnums;
using Thinktecture.Runtime.Tests.Text.Json.Serialization.ValueObjectJsonConverterFactoryTests.TestClasses;
using Thinktecture.Text.Json.Serialization;

namespace Thinktecture.Runtime.Tests.Text.Json.Serialization.ValueObjectJsonConverterFactoryTests;

public class JsonTestsBase
{
   public static IEnumerable<object[]> DataForStringBasedEnumTest =>
   [
      [null, "null"],
      [TestEnum.Item1, "\"item1\""],
      [TestEnum.Item2, "\"item2\""]
   ];

   public static IEnumerable<object[]> DataForClassWithStringBasedEnumTest =>
   [
      [null, "null"],
      [new ClassWithStringBasedEnum(), "{}", true],
      [new ClassWithStringBasedEnum(), "{\"Enum\":null}"],
      [new ClassWithStringBasedEnum(TestEnum.Item1), "{\"Enum\":\"item1\"}"],
      [new ClassWithStringBasedEnum(TestEnum.Item2), "{\"Enum\":\"item2\"}"]
   ];

   public static IEnumerable<object[]> DataForIntBasedEnumTest =>
   [
      [null, "null"],
      [IntegerEnum.Item1, "1"],
      [IntegerEnum.Item2, "2"]
   ];

   public static IEnumerable<object[]> DataForClassWithIntBasedEnumTest =>
   [
      [null, "null"],
      [new ClassWithIntBasedEnum(), "{}", true],
      [new ClassWithIntBasedEnum(), "{\"Enum\":null}"],
      [new ClassWithIntBasedEnum(IntegerEnum.Item1), "{\"Enum\":1}"],
      [new ClassWithIntBasedEnum(IntegerEnum.Item2), "{\"Enum\":2}"]
   ];

   protected static string Serialize<T, TKey>(
      T value,
      JsonNamingPolicy namingStrategy = null,
      bool ignoreNullValues = false,
      JsonNumberHandling numberHandling = JsonNumberHandling.Strict)
      where T : IValueObjectFactory<T, TKey, ValidationError>, IValueObjectConvertable<TKey>
   {
      return Serialize<T, TKey, ValidationError>(value, namingStrategy, ignoreNullValues, numberHandling);
   }

   protected static string Serialize<T, TKey, TValidationError>(
      T value,
      JsonNamingPolicy namingStrategy = null,
      bool ignoreNullValues = false,
      JsonNumberHandling numberHandling = JsonNumberHandling.Strict)
      where T : IValueObjectFactory<T, TKey, TValidationError>, IValueObjectConvertable<TKey>
      where TValidationError : class, IValidationError<TValidationError>
   {
      return SerializeWithConverter<T, ValueObjectJsonConverterFactory>(value, namingStrategy, ignoreNullValues, numberHandling);
   }

   protected static string SerializeWithConverter<T, TConverterFactory>(
      T value,
      JsonNamingPolicy namingPolicy = null,
      bool ignoreNullValues = false,
      JsonNumberHandling numberHandling = JsonNumberHandling.Strict)
      where TConverterFactory : JsonConverterFactory, new()
   {
      return SerializeWithConverter<T, TConverterFactory>(value, namingPolicy, ignoreNullValues ? JsonIgnoreCondition.WhenWritingNull : JsonIgnoreCondition.Never, numberHandling);
   }

   protected static string SerializeWithConverter<T, TConverterFactory>(
      T value,
      JsonNamingPolicy namingPolicy,
      JsonIgnoreCondition jsonIgnoreCondition,
      JsonNumberHandling numberHandling = JsonNumberHandling.Strict)
      where TConverterFactory : JsonConverterFactory, new()
   {
      var factory = new TConverterFactory();
      var options = new JsonSerializerOptions
                    {
                       Converters = { factory },
                       PropertyNamingPolicy = namingPolicy,
                       DefaultIgnoreCondition = jsonIgnoreCondition,
                       NumberHandling = numberHandling
                    };

      return JsonSerializer.Serialize(value, options);
   }

   protected static T Deserialize<T>(
      string json,
      JsonNamingPolicy namingPolicy = null,
      bool propertyNameCaseInsensitive = false,
      bool ignoreNullValues = false,
      JsonNumberHandling numberHandling = JsonNumberHandling.Strict)
   {
      var options = new JsonSerializerOptions
                    {
                       PropertyNamingPolicy = namingPolicy,
                       PropertyNameCaseInsensitive = propertyNameCaseInsensitive,
                       DefaultIgnoreCondition = ignoreNullValues ? JsonIgnoreCondition.WhenWritingNull : JsonIgnoreCondition.Never,
                       NumberHandling = numberHandling
                    };

      return JsonSerializer.Deserialize<T>(json, options);
   }
}

using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Thinktecture.Runtime.Tests.TestEnums;
using Thinktecture.Runtime.Tests.Text.Json.Serialization.ValueObjectJsonConverterFactoryTests.TestClasses;
using Thinktecture.Text.Json.Serialization;

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

   protected static T Deserialize<T, TKey, TValidationError>(
      string json,
      JsonNamingPolicy namingPolicy = null)
      where T : IValueObjectFactory<T, TKey, TValidationError>, IValueObjectConvertable<TKey>
      where TValidationError : class, IValidationError<TValidationError>
   {
      return DeserializeWithConverter<T, ValueObjectJsonConverterFactory>(json, namingPolicy);
   }

   protected static T Deserialize<T, TKey>(
      string json,
      JsonNamingPolicy namingPolicy = null,
      JsonNumberHandling numberHandling = JsonNumberHandling.Strict)
      where T : IValueObjectFactory<T, TKey, ValidationError>, IValueObjectConvertable<TKey>
   {
      return DeserializeWithConverter<T, ValueObjectJsonConverterFactory>(json, namingPolicy, numberHandling: numberHandling);
   }

   protected static T DeserializeWithConverter<T, TConverterFactory>(
      string json,
      JsonNamingPolicy namingPolicy = null,
      bool propertyNameCaseInsensitive = false,
      bool ignoreNullValues = false,
      JsonNumberHandling numberHandling = JsonNumberHandling.Strict)
      where TConverterFactory : JsonConverterFactory, new()
   {
      var factory = new TConverterFactory();
      var options = new JsonSerializerOptions
                    {
                       Converters = { factory },
                       PropertyNamingPolicy = namingPolicy,
                       PropertyNameCaseInsensitive = propertyNameCaseInsensitive,
                       DefaultIgnoreCondition = ignoreNullValues ? JsonIgnoreCondition.WhenWritingNull : JsonIgnoreCondition.Never,
                       NumberHandling = numberHandling
                    };

      return JsonSerializer.Deserialize<T>(json, options);
   }
}

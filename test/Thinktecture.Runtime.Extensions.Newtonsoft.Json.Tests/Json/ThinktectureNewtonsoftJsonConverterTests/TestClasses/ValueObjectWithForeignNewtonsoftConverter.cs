using System;
using Newtonsoft.Json;

namespace Thinktecture.Runtime.Tests.Json.ThinktectureNewtonsoftJsonConverterTests.TestClasses;

// Regression type: an int-keyed Value Object that carries a foreign Newtonsoft [JsonConverter] attribute. A factory
// configured to skip value objects with such an attribute must refuse this type. See the shared-cache regression test.
[ValueObject<int>]
[JsonConverter(typeof(ForeignNewtonsoftConverter))]
public partial class ValueObjectWithForeignNewtonsoftConverter
{
}

public sealed class ForeignNewtonsoftConverter : JsonConverter
{
   public override bool CanConvert(Type objectType)
   {
      return objectType == typeof(ValueObjectWithForeignNewtonsoftConverter);
   }

   public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
   {
      writer.WriteValue("foreign");
   }

   public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
   {
      throw new NotSupportedException();
   }
}

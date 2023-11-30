using System.Text.Json;
using System.Text.Json.Serialization;

namespace Thinktecture.Text.Json.Serialization;

internal sealed class SingleKeyConverter : JsonConverter<float>
{
   public static readonly SingleKeyConverter Instance = new();

   private SingleKeyConverter()
   {
   }

   public override float Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
   {
      return reader.GetSingleWithNumberHandling(options.NumberHandling);
   }

   public override void Write(Utf8JsonWriter writer, float value, JsonSerializerOptions options)
   {
      writer.WriteNumberWithNumberHandling(value, options.NumberHandling);
   }
}

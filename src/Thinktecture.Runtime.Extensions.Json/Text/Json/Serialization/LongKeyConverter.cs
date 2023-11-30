using System.Text.Json;
using System.Text.Json.Serialization;

namespace Thinktecture.Text.Json.Serialization;

internal sealed class LongKeyConverter : JsonConverter<long>
{
   public static readonly LongKeyConverter Instance = new();

   private LongKeyConverter()
   {
   }

   public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
   {
      return reader.GetLongWithNumberHandling(options.NumberHandling);
   }

   public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options)
   {
      writer.WriteNumberWithNumberHandling(value, options.NumberHandling);
   }
}

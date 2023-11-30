using System.Text.Json;
using System.Text.Json.Serialization;

namespace Thinktecture.Text.Json.Serialization;

internal sealed class IntKeyConverter : JsonConverter<int>
{
   public static readonly IntKeyConverter Instance = new();

   private IntKeyConverter()
   {
   }

   public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
   {
      return reader.GetIntWithNumberHandling(options.NumberHandling);
   }

   public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
   {
      writer.WriteNumberWithNumberHandling(value, options.NumberHandling);
   }
}

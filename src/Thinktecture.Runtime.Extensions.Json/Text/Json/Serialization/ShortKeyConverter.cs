using System.Text.Json;
using System.Text.Json.Serialization;

namespace Thinktecture.Text.Json.Serialization;

internal sealed class ShortKeyConverter : JsonConverter<short>
{
   public static readonly ShortKeyConverter Instance = new();

   private ShortKeyConverter()
   {
   }

   public override short Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
   {
      return reader.GetShortWithNumberHandling(options.NumberHandling);
   }

   public override void Write(Utf8JsonWriter writer, short value, JsonSerializerOptions options)
   {
      writer.WriteNumberWithNumberHandling(value, options.NumberHandling);
   }
}

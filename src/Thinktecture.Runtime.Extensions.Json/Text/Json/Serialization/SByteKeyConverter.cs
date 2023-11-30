using System.Text.Json;
using System.Text.Json.Serialization;

namespace Thinktecture.Text.Json.Serialization;

internal sealed class SByteKeyConverter : JsonConverter<sbyte>
{
   public static readonly SByteKeyConverter Instance = new();

   private SByteKeyConverter()
   {
   }

   public override sbyte Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
   {
      return reader.GetSByteWithNumberHandling(options.NumberHandling);
   }

   public override void Write(Utf8JsonWriter writer, sbyte value, JsonSerializerOptions options)
   {
      writer.WriteNumberWithNumberHandling(value, options.NumberHandling);
   }
}

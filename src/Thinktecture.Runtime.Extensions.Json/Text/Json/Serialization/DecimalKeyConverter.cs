using System.Text.Json;
using System.Text.Json.Serialization;

namespace Thinktecture.Text.Json.Serialization;

internal sealed class DecimalKeyConverter : JsonConverter<decimal>
{
   public static readonly DecimalKeyConverter Instance = new();

   private DecimalKeyConverter()
   {
   }

   public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
   {
      return reader.GetDecimalWithNumberHandling(options.NumberHandling);
   }

   public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
   {
      writer.WriteNumberWithNumberHandling(value, options.NumberHandling);
   }
}

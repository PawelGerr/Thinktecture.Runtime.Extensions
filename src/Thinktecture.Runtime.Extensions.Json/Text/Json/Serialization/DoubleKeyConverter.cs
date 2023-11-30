using System.Text.Json;
using System.Text.Json.Serialization;

namespace Thinktecture.Text.Json.Serialization;

internal sealed class DoubleKeyConverter : JsonConverter<double>
{
   public static readonly DoubleKeyConverter Instance = new();

   private DoubleKeyConverter()
   {
   }

   public override double Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
   {
      return reader.GetDoubleWithNumberHandling(options.NumberHandling);
   }

   public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options)
   {
      writer.WriteNumberWithNumberHandling(value, options.NumberHandling);
   }
}

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Thinktecture.Text.Json.Serialization;

internal sealed class UShortKeyConverter : JsonConverter<ushort>
{
   public static readonly UShortKeyConverter Instance = new();

   private UShortKeyConverter()
   {
   }

   public override ushort Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
   {
      return reader.GetUShortWithNumberHandling(options.NumberHandling);
   }

   public override void Write(Utf8JsonWriter writer, ushort value, JsonSerializerOptions options)
   {
      writer.WriteNumberWithNumberHandling(value, options.NumberHandling);
   }
}

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Thinktecture.Text.Json.Serialization;

internal sealed class ULongKeyConverter : JsonConverter<ulong>
{
   public static readonly ULongKeyConverter Instance = new();

   private ULongKeyConverter()
   {
   }

   public override ulong Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
   {
      return reader.GetULongWithNumberHandling(options.NumberHandling);
   }

   public override void Write(Utf8JsonWriter writer, ulong value, JsonSerializerOptions options)
   {
      writer.WriteNumberWithNumberHandling(value, options.NumberHandling);
   }
}

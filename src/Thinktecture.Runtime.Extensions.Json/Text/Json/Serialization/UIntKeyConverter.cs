using System.Text.Json;
using System.Text.Json.Serialization;

namespace Thinktecture.Text.Json.Serialization;

internal sealed class UIntKeyConverter : JsonConverter<uint>
{
   public static readonly UIntKeyConverter Instance = new();

   private UIntKeyConverter()
   {
   }

   public override uint Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
   {
      return reader.GetUIntWithNumberHandling(options.NumberHandling);
   }

   public override void Write(Utf8JsonWriter writer, uint value, JsonSerializerOptions options)
   {
      writer.WriteNumberWithNumberHandling(value, options.NumberHandling);
   }
}

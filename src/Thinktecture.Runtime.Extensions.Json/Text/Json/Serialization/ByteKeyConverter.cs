using System.Text.Json;
using System.Text.Json.Serialization;

namespace Thinktecture.Text.Json.Serialization;

internal sealed class ByteKeyConverter : JsonConverter<byte>
{
   public static readonly ByteKeyConverter Instance = new();

   private ByteKeyConverter()
   {
   }

   public override byte Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
   {
      return reader.GetByteWithNumberHandling(options.NumberHandling);
   }

   public override void Write(Utf8JsonWriter writer, byte value, JsonSerializerOptions options)
   {
      writer.WriteNumberWithNumberHandling(value, options.NumberHandling);
   }
}

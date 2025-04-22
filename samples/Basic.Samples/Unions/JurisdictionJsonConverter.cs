using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Thinktecture.Unions;

public partial class JurisdictionJsonConverter : JsonConverter<Jurisdiction>
{
   public override Jurisdiction? Read(
      ref Utf8JsonReader reader,
      Type typeToConvert,
      JsonSerializerOptions options)
   {
      if (!reader.Read()    // read StartObject
          || !reader.Read() // read PropertyName
          || !Discriminator.TryGet(reader.GetString(), out var discriminator))
         throw new JsonException();

      var jurisdiction = discriminator.ReadJurisdiction(ref reader, options);

      if (!reader.Read()) // read EndObject
         throw new JsonException();

      return jurisdiction;
   }

   public override void Write(
      Utf8JsonWriter writer,
      Jurisdiction value,
      JsonSerializerOptions options)
   {
      value.Switch(
         (writer, options),
         country: static (state, country) =>
            WriteJurisdiction(state.writer, state.options, country, Discriminator.Country),
         federalState: static (state, federalState) =>
            WriteJurisdiction(state.writer, state.options, federalState, Discriminator.FederalState),
         district: static (state, district) =>
            WriteJurisdiction(state.writer, state.options, district, Discriminator.District),
         unknown: static (state, unknown) =>
            WriteJurisdiction(state.writer, state.options, unknown, Discriminator.Unknown)
      );
   }

   private static void WriteJurisdiction<T>(
      Utf8JsonWriter writer,
      JsonSerializerOptions options,
      T jurisdiction,
      string discriminator
   )
      where T : Jurisdiction
   {
      writer.WriteStartObject();

      writer.WriteString("$type", discriminator);

      writer.WritePropertyName(options.PropertyNamingPolicy?.ConvertName("value") ?? "value");
      JsonSerializer.Serialize(writer, jurisdiction, options);

      writer.WriteEndObject();
   }

   [SmartEnum<string>]
   internal partial class Discriminator
   {
      public static readonly Discriminator Country = new("Country", ReadJurisdiction<Jurisdiction.Country>);
      public static readonly Discriminator FederalState = new("FederalState", ReadJurisdiction<Jurisdiction.FederalState>);
      public static readonly Discriminator District = new("District", ReadJurisdiction<Jurisdiction.District>);
      public static readonly Discriminator Unknown = new("Unknown", ReadJurisdiction<Jurisdiction.Unknown>);

      [UseDelegateFromConstructor]
      public partial Jurisdiction? ReadJurisdiction(ref Utf8JsonReader reader, JsonSerializerOptions options);

      private static Jurisdiction? ReadJurisdiction<T>(
         ref Utf8JsonReader reader,
         JsonSerializerOptions options)
         where T : Jurisdiction
      {
         if (!reader.Read() || !reader.Read()) // read PropertyName and value
            throw new JsonException();

         return JsonSerializer.Deserialize<T>(ref reader, options);
      }
   }
}

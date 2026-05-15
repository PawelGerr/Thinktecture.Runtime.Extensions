#nullable enable
using System;
using System.Text.Json;
using Thinktecture.Text.Json.Serialization;

namespace Thinktecture.Runtime.Tests.Text.Json.Serialization.ThinktectureJsonConverterFactoryTests;

// ReSharper disable InconsistentNaming
public class NormalizeMemberRoundTrip
{
   [Fact]
   public void Should_apply_NormalizeMember_during_JSON_deserialization_round_trip()
   {
      var options = new JsonSerializerOptions { Converters = { new ThinktectureJsonConverterFactory() } };

      var json = "\"  Hello  \"";

      var deserialized = JsonSerializer.Deserialize<NormalizingSerializableUnion>(json, options);

      deserialized.Should().NotBeNull();
      deserialized!.AsString.Should().Be("hello");
   }

   [Fact]
   public void Should_apply_NormalizeMember_during_JSON_serialize_then_deserialize()
   {
      var options = new JsonSerializerOptions { Converters = { new ThinktectureJsonConverterFactory() } };

      var original = new NormalizingSerializableUnion("  WORLD  "); // ctor normalizes to "world"
      var json = JsonSerializer.Serialize(original, options);

      json.Should().Be("\"world\"");

      var roundTripped = JsonSerializer.Deserialize<NormalizingSerializableUnion>(json, options);
      roundTripped.Should().Be(original);
   }
}

[Union<string, int>]
[ObjectFactory<string>(UseForSerialization = SerializationFrameworks.All)]
public partial class NormalizingSerializableUnion
{
   public string ToValue()
   {
      return Switch(@string: t => t,
                    @int32: n => n.ToString(System.Globalization.CultureInfo.InvariantCulture));
   }

   public static ValidationError? Validate(string? value, IFormatProvider? provider, out NormalizingSerializableUnion? item)
   {
      if (String.IsNullOrWhiteSpace(value))
      {
         item = null;
         return null;
      }

      item = value!;
      return null;
   }

   static partial void NormalizeString(ref string @string)
   {
      @string = @string.Trim().ToLowerInvariant();
   }
}

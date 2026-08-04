#nullable enable
using System;
using System.Text.Json;
using Thinktecture.Text.Json.Serialization;

namespace Thinktecture.Runtime.Tests.Text.Json.Serialization.ThinktectureJsonConverterFactoryTests;

// ReSharper disable InconsistentNaming
public class RenamedValueUnionRoundTrip
{
   [Fact]
   public void Should_round_trip_union_when_Value_property_is_renamed()
   {
      var options = new JsonSerializerOptions { Converters = { new ThinktectureJsonConverterFactory() } };

      var original = new RenamedValueSerializableUnion("hello");
      var json = JsonSerializer.Serialize(original, options);

      json.Should().Be("\"hello\"");

      var roundTripped = JsonSerializer.Deserialize<RenamedValueSerializableUnion>(json, options);
      roundTripped.Should().Be(original);
   }
}

// The union renames its raw-value property to RawValue but is still serializable, because
// serialization goes through the metadata delegates and the object factory, not through the Value
// property by name.
[Union<string, int>(ValueMemberName = "RawValue")]
[ObjectFactory<string>(UseForSerialization = SerializationFrameworks.All)]
public partial class RenamedValueSerializableUnion
{
   public string ToValue()
   {
      return Switch(@string: t => t,
                    @int32: n => n.ToString(System.Globalization.CultureInfo.InvariantCulture));
   }

   public static ValidationError? Validate(string? value, IFormatProvider? provider, out RenamedValueSerializableUnion? item)
   {
      if (String.IsNullOrWhiteSpace(value))
      {
         item = null;
         return null;
      }

      item = value!;
      return null;
   }
}

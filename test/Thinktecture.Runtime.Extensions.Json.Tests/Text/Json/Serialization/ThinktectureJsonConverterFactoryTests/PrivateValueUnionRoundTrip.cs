#nullable enable
using System;
using System.Text.Json;
using Thinktecture.Text.Json.Serialization;

namespace Thinktecture.Runtime.Tests.Text.Json.Serialization.ThinktectureJsonConverterFactoryTests;

// ReSharper disable InconsistentNaming
public class PrivateValueUnionRoundTrip
{
   [Fact]
   public void Should_round_trip_union_when_Value_property_is_private()
   {
      var options = new JsonSerializerOptions { Converters = { new ThinktectureJsonConverterFactory() } };

      var original = new PrivateValueSerializableUnion("hello");
      var json = JsonSerializer.Serialize(original, options);

      json.Should().Be("\"hello\"");

      var roundTripped = JsonSerializer.Deserialize<PrivateValueSerializableUnion>(json, options);
      roundTripped.Should().Be(original);
   }
}

// The union hides its raw Value property but is still serializable, because serialization goes
// through the metadata delegates and the object factory, not through the Value property by name.
[Union<string, int>(ValueMemberAccessModifier = AccessModifier.Private)]
[ObjectFactory<string>(UseForSerialization = SerializationFrameworks.All)]
public partial class PrivateValueSerializableUnion
{
   public string ToValue()
   {
      return Switch(@string: t => t,
                    @int32: n => n.ToString(System.Globalization.CultureInfo.InvariantCulture));
   }

   public static ValidationError? Validate(string? value, IFormatProvider? provider, out PrivateValueSerializableUnion? item)
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

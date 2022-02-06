using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.SourceGeneratorTests.NewtonsoftJson;

public class ReadJson
{
   [Fact]
   public void Should_throw_JsonSerializationException_if_enum_parsing_throws_UnknownEnumIdentifierException()
   {
      Action action = () => Deserialize<ValidTestEnum>("\"invalid\"");

      action.Should().Throw<JsonSerializationException>().WithMessage("There is no item of type 'ValidTestEnum' with the identifier 'invalid'.");
   }

   private static T Deserialize<T>(
      string json,
      NamingStrategy namingStrategy = null)
   {
      using var reader = new StringReader(json);
      using var jsonReader = new JsonTextReader(reader);

      var settings = new JsonSerializerSettings();

      if (namingStrategy is not null)
         settings.ContractResolver = new DefaultContractResolver { NamingStrategy = namingStrategy };

      var serializer = JsonSerializer.CreateDefault(settings);

      return serializer.Deserialize<T>(jsonReader);
   }
}

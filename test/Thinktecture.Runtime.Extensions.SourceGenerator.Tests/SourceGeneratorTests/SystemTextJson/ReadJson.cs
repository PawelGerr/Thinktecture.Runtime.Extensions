using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using Thinktecture.Runtime.Tests.TestEnums;
using Xunit;

namespace Thinktecture.Runtime.Tests.SourceGeneratorTests.SystemTextJson
{
   public class ReadJson
   {
      [Fact]
      public void Should_throw_JsonException_if_enum_parsing_throws_UnknownEnumIdentifierException()
      {
         Action action = () => Deserialize<ValidTestEnum>("\"invalid\"");

         action.Should().Throw<JsonException>().WithMessage("There is no item of type 'ValidTestEnum' with the identifier 'invalid'.");
      }

      private static T Deserialize<T>(
         string json,
         JsonNamingPolicy namingPolicy = null,
         bool propertyNameCaseInsensitive = false,
         bool ignoreNullValues = false)
      {
         var options = new JsonSerializerOptions
                       {
                          PropertyNamingPolicy = namingPolicy,
                          PropertyNameCaseInsensitive = propertyNameCaseInsensitive
                       };

         if (ignoreNullValues)
            options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

         return JsonSerializer.Deserialize<T>(json, options);
      }
   }
}

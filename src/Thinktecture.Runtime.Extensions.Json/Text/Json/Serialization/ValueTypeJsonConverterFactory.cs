using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Thinktecture.Text.Json.Serialization
{
   /// <summary>
   /// Factory for creation of <see cref="ValueTypeJsonConverter{T,TKey}"/>.
   /// </summary>
   public class ValueTypeJsonConverterFactory : JsonConverterFactory
   {
      /// <inheritdoc />
      public override bool CanConvert(Type typeToConvert)
      {
         return ValueTypeMetadataLookup.Find(typeToConvert) is not null;
      }

      /// <inheritdoc />
      public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
      {
         if (typeToConvert is null)
            throw new ArgumentNullException(nameof(typeToConvert));
         if (options is null)
            throw new ArgumentNullException(nameof(options));

         var metadata = ValueTypeMetadataLookup.Find(typeToConvert);

         if (metadata is null)
            throw new InvalidOperationException($"No metadata for provided type '{typeToConvert.Name}' found.");

         var keyConverter = options.GetConverter(metadata.KeyType);

         if (keyConverter is null)
            throw new ArgumentException($"The type '{typeToConvert.Name}' is not JSON-serializable because there is no {nameof(JsonConverter)} for its key member of type '{metadata.KeyType.Name}'.", nameof(typeToConvert));

         var converterType = typeof(ValueTypeJsonConverter<,>).MakeGenericType(metadata.Type, metadata.KeyType);
         var converter = Activator.CreateInstance(converterType, metadata.ConvertFromKey, metadata.ConvertToKey, keyConverter)
                         ?? throw new Exception($"Could not create converter of type '{converterType.Name}'.");

         return (JsonConverter)converter;
      }
   }
}

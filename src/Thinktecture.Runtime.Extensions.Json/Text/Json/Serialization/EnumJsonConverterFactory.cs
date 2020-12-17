using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Thinktecture.Text.Json.Serialization
{
   /// <summary>
   /// Factory for creation of <see cref="EnumJsonConverter{TEnum,TKey}"/>.
   /// </summary>
   public class EnumJsonConverterFactory : JsonConverterFactory
   {
      /// <inheritdoc />
      public override bool CanConvert(Type typeToConvert)
      {
         return EnumMetadataLookup.FindEnum(typeToConvert) is not null;
      }

      /// <inheritdoc />
      public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
      {
         if (typeToConvert is null)
            throw new ArgumentNullException(nameof(typeToConvert));
         if (options is null)
            throw new ArgumentNullException(nameof(options));

         var enumMetadata = EnumMetadataLookup.FindEnum(typeToConvert);

         if (enumMetadata is null)
            throw new InvalidOperationException($"No metadata for provided type '{typeToConvert.Name}' found.");

         var keyConverter = options.GetConverter(enumMetadata.KeyType);

         if (keyConverter is null)
            throw new ArgumentException($"The enum '{typeToConvert.Name}' is not JSON-serializable because there is no {nameof(JsonConverter)} for its key of type '{enumMetadata.KeyType.Name}'.", nameof(typeToConvert));

         var converterType = typeof(EnumJsonConverter<,>).MakeGenericType(enumMetadata.EnumType, enumMetadata.KeyType);
         var converter = Activator.CreateInstance(converterType, enumMetadata.ConvertFromKey, keyConverter)
                         ?? throw new Exception($"Could not create converter of type '{converterType.Name}'.");

         return (JsonConverter)converter;
      }
   }
}

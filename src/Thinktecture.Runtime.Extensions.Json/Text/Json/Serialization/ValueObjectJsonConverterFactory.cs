using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Thinktecture.Internal;

namespace Thinktecture.Text.Json.Serialization
{
   /// <summary>
   /// Factory for creation of <see cref="ValueObjectJsonConverter{T,TKey}"/>.
   /// </summary>
   public class ValueObjectJsonConverterFactory : JsonConverterFactory
   {
      /// <inheritdoc />
      public override bool CanConvert(Type typeToConvert)
      {
         return ValueObjectMetadataLookup.Find(typeToConvert) is not null;
      }

      /// <inheritdoc />
      public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
      {
         if (typeToConvert is null)
            throw new ArgumentNullException(nameof(typeToConvert));
         if (options is null)
            throw new ArgumentNullException(nameof(options));

         var metadata = ValueObjectMetadataLookup.Find(typeToConvert);

         if (metadata is null)
            throw new InvalidOperationException($"No metadata for provided type '{typeToConvert.Name}' found.");

         var converterType = typeof(ValueObjectJsonConverter<,>).MakeGenericType(metadata.Type, metadata.KeyType);
         var converter = Activator.CreateInstance(converterType, metadata.ConvertFromKey, metadata.ConvertToKey, options)
                         ?? throw new Exception($"Could not create converter of type '{converterType.Name}'.");

         return (JsonConverter)converter;
      }
   }
}

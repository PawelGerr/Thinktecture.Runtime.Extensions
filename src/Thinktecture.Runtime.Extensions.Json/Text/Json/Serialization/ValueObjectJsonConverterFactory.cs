using System.Text.Json;
using System.Text.Json.Serialization;
using Thinktecture.Internal;

namespace Thinktecture.Text.Json.Serialization;

/// <summary>
/// Factory for creation of <see cref="ValueObjectJsonConverter{T,TKey}"/>.
/// </summary>
public sealed class ValueObjectJsonConverterFactory : JsonConverterFactory
{
   /// <inheritdoc />
   public override bool CanConvert(Type typeToConvert)
   {
#if !NET7_0
      return ValueObjectMetadataLookup.Find(typeToConvert) is not null;
#else
      return typeToConvert.GetInterfaces()
                          .Any(static i => i.IsGenericType && !i.IsGenericTypeDefinition && i.GetGenericTypeDefinition() == typeof(IKeyedValueObject<,>));
#endif
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

#if !NET7_0
      var converter = Activator.CreateInstance(converterType, metadata.ConvertFromKey, options);
#else
      var converter = Activator.CreateInstance(converterType, metadata.IsValidatableEnum, options);
#endif

      return (JsonConverter)(converter ?? throw new Exception($"Could not create converter of type '{converterType.Name}'."));
   }
}

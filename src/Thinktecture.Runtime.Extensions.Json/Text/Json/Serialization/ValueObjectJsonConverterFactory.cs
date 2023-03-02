using System.Text.Json;
using System.Text.Json.Serialization;
using Thinktecture.Internal;

namespace Thinktecture.Text.Json.Serialization;

/// <summary>
/// Factory for creation of <see cref="ValueObjectJsonConverter{T,TKey}"/>.
/// </summary>
public sealed class ValueObjectJsonConverterFactory<T, TKey> : JsonConverterFactory
   where T : IKeyedValueObject<T, TKey>
   where TKey : notnull
{
   /// <inheritdoc />
   public override bool CanConvert(Type typeToConvert)
   {
      return typeof(T).IsAssignableFrom(typeToConvert);
   }

   /// <inheritdoc />
   public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
   {
      ArgumentNullException.ThrowIfNull(typeToConvert);
      ArgumentNullException.ThrowIfNull(options);

      return new ValueObjectJsonConverter<T, TKey>(options);
   }
}

/// <summary>
/// Factory for creation of <see cref="ValueObjectJsonConverter{T,TKey}"/>.
/// </summary>
public sealed class ValueObjectJsonConverterFactory : JsonConverterFactory
{
   /// <inheritdoc />
   public override bool CanConvert(Type typeToConvert)
   {
      return KeyedValueObjectMetadataLookup.Find(typeToConvert) is not null;
   }

   /// <inheritdoc />
   public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
   {
      ArgumentNullException.ThrowIfNull(typeToConvert);
      ArgumentNullException.ThrowIfNull(options);

      var metadata = KeyedValueObjectMetadataLookup.Find(typeToConvert);

      if (metadata is null)
         throw new InvalidOperationException($"No metadata for provided type '{typeToConvert.Name}' found.");

      var converterType = typeof(ValueObjectJsonConverter<,>).MakeGenericType(metadata.Type, metadata.KeyType);
      var converter = Activator.CreateInstance(converterType, options);

      return (JsonConverter)(converter ?? throw new Exception($"Could not create converter of type '{converterType.Name}'."));
   }
}

using System.Collections.Concurrent;
using Newtonsoft.Json;
using Thinktecture.Internal;

namespace Thinktecture.Json;

/// <summary>
/// <see cref="JsonConverter"/> for Value Objects.
/// </summary>
/// <typeparam name="T">Type of the value object.</typeparam>
/// <typeparam name="TKey">Type of the key.</typeparam>
public sealed class ValueObjectNewtonsoftJsonConverter<T, TKey> : ValueObjectNewtonsoftJsonConverterBase<T, TKey>
   where TKey : notnull
{
   /// <summary>
   /// Initializes a new instance of <see cref="ValueObjectNewtonsoftJsonConverter{T,TKey}"/>.
   /// </summary>
   /// <param name="convertFromKey">Converts an instance of type <typeparamref name="TKey"/> to an instance of <typeparamref name="T"/>.</param>
   /// <param name="convertToKey">Converts an instance of type <typeparamref name="T"/> to an instance of <typeparamref name="TKey"/>.</param>
   public ValueObjectNewtonsoftJsonConverter(Func<TKey, T> convertFromKey, Func<T, TKey> convertToKey)
      : base(convertFromKey, convertToKey)
   {
   }
}

/// <summary>
/// Non-generic converter for Value Objects.
/// </summary>
public sealed class ValueObjectNewtonsoftJsonConverter : JsonConverter
{
   private static readonly ConcurrentDictionary<Type, JsonConverter> _cache = new();

   /// <inheritdoc />
   public override bool CanConvert(Type objectType)
   {
      return _cache.ContainsKey(objectType) || ValueObjectMetadataLookup.Find(objectType) is not null;
   }

   /// <inheritdoc />
   public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
   {
      if (writer is null)
         throw new ArgumentNullException(nameof(writer));

      if (value is null)
      {
         writer.WriteNull();
      }
      else
      {
         var converter = _cache.GetOrAdd(value.GetType(), CreateConverter);
         converter.WriteJson(writer, value, serializer);
      }
   }

   /// <inheritdoc />
   public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
   {
      var converter = _cache.GetOrAdd(objectType, CreateConverter);

      return converter.ReadJson(reader, objectType, existingValue, serializer);
   }

   private static JsonConverter CreateConverter(Type type)
   {
      var metadata = ValueObjectMetadataLookup.Find(type);

      if (metadata is null)
         throw new InvalidOperationException($"The provided type is not serializable by the '{nameof(ValueObjectNewtonsoftJsonConverter)}'. Type: {type.FullName}");

      var converterType = typeof(ValueObjectNewtonsoftJsonConverter<,>).MakeGenericType(metadata.Type, metadata.KeyType);
      var converter = Activator.CreateInstance(converterType, metadata.ConvertFromKey, metadata.ConvertToKey)
                      ?? throw new Exception($"Could not create a converter of type '{converterType.Name}'.");

      return (JsonConverter)converter;
   }
}

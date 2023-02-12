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
   where T : IKeyedValueObject<T, TKey>
   where TKey : notnull
{
   /// <summary>
   /// Initializes a new instance of <see cref="ValueObjectNewtonsoftJsonConverter{T,TKey}"/>.
   /// </summary>
   /// <param name="mayReturnInvalidObjects">Indication whether invalid should be returned on deserialization. If <c>false</c> then a <see cref="JsonException"/> is thrown.</param>
   public ValueObjectNewtonsoftJsonConverter(bool mayReturnInvalidObjects)
      : base(mayReturnInvalidObjects)
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
      return _cache.ContainsKey(objectType) || KeyedValueObjectMetadataLookup.Find(objectType) is not null;
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
      var metadata = KeyedValueObjectMetadataLookup.Find(type);

      if (metadata is null)
         throw new InvalidOperationException($"The provided type is not serializable by the '{nameof(ValueObjectNewtonsoftJsonConverter)}'. Type: {type.FullName}");

      var converterType = typeof(ValueObjectNewtonsoftJsonConverter<,>).MakeGenericType(metadata.Type, metadata.KeyType);
      var converter = Activator.CreateInstance(converterType, new object?[] { metadata.IsValidatableEnum });

      return (JsonConverter)(converter ?? throw new Exception($"Could not create converter of type '{converterType.Name}'."));
   }
}

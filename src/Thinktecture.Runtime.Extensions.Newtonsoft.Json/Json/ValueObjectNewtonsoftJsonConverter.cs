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
   where T : IKeyedValueObject<TKey>
#if NET7_0
 , IKeyedValueObject<T, TKey>
#endif
   where TKey : notnull
{
#if NET7_0
   /// <summary>
   /// Initializes a new instance of <see cref="ValueObjectNewtonsoftJsonConverter{T,TKey}"/>.
   /// </summary>
   /// <param name="mayReturnInvalidObjects">Indication whether invalid should be returned on deserialization. If <c>false</c> then a <see cref="JsonException"/> is thrown.</param>
   public ValueObjectNewtonsoftJsonConverter(bool mayReturnInvalidObjects)
      : base(mayReturnInvalidObjects)
   {
   }
#else
   /// <summary>
   /// Initializes a new instance of <see cref="ValueObjectNewtonsoftJsonConverter{T,TKey}"/>.
   /// </summary>
   /// <param name="convertFromKey">Converts an instance of type <typeparamref name="TKey"/> to an instance of <typeparamref name="T"/>.</param>
   public ValueObjectNewtonsoftJsonConverter(Func<TKey, T> convertFromKey)
      : base(convertFromKey)
   {
   }
#endif
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

#if NET7_0
      var converter = Activator.CreateInstance(converterType, new object?[] { metadata.IsValidatableEnum });
#else
      var converter = Activator.CreateInstance(converterType, metadata.ConvertFromKey);
#endif

      return (JsonConverter)(converter ?? throw new Exception($"Could not create converter of type '{converterType.Name}'."));
   }
}

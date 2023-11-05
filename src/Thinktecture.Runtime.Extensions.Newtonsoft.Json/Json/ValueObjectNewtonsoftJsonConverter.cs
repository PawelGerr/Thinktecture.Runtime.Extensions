using System.Collections.Concurrent;
using System.Reflection;
using Newtonsoft.Json;
using Thinktecture.Internal;

namespace Thinktecture.Json;

/// <summary>
/// Non-generic converter for Value Objects.
/// </summary>
public sealed class ValueObjectNewtonsoftJsonConverter : JsonConverter
{
   private static readonly ConcurrentDictionary<Type, JsonConverter> _cache = new();

   /// <inheritdoc />
   public override bool CanConvert(Type objectType)
   {
      return _cache.ContainsKey(objectType)
             || KeyedValueObjectMetadataLookup.Find(objectType) is not null
             || objectType.GetCustomAttributes<ValueObjectFactoryAttribute>().Any(a => a.UseForSerialization.HasFlag(SerializationFrameworks.NewtonsoftJson));
   }

   /// <inheritdoc />
   public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
   {
      ArgumentNullException.ThrowIfNull(writer);

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
      var customFactory = type.GetCustomAttributes<ValueObjectFactoryAttribute>()
                              .LastOrDefault(a => a.UseForSerialization.HasFlag(SerializationFrameworks.NewtonsoftJson));

      Type modelType;
      Type keyType;

      if (customFactory is not null)
      {
         modelType = type;
         keyType = customFactory.Type;
      }
      else if (metadata is not null)
      {
         modelType = metadata.Type;
         keyType = metadata.KeyType;
      }
      else
      {
         throw new InvalidOperationException($"The provided type is not serializable by the '{nameof(ValueObjectNewtonsoftJsonConverter)}'. Type: {type.FullName}");
      }

      var converterType = typeof(ValueObjectNewtonsoftJsonConverter<,>).MakeGenericType(modelType, keyType);
      var converter = Activator.CreateInstance(converterType);

      return (JsonConverter)(converter ?? throw new Exception($"Could not create converter of type '{converterType.Name}'."));
   }
}

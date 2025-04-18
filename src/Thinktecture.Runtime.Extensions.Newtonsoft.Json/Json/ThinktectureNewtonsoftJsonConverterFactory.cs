using System.Collections.Concurrent;
using System.Reflection;
using Newtonsoft.Json;
using Thinktecture.Internal;

namespace Thinktecture.Json;

/// <summary>
/// Non-generic converter for Value Objects.
/// </summary>
[Obsolete("Use 'jsonSerializerSettings.AddThinktectureJsonConverters()' instead.")]
public sealed class ValueObjectNewtonsoftJsonConverter : ThinktectureNewtonsoftJsonConverterFactory;

/// <summary>
/// Non-generic converter for Value Objects.
/// </summary>
[ThinktectureRuntimeExtensionInternal]
public class ThinktectureNewtonsoftJsonConverterFactory : JsonConverter
{
   private static readonly ConcurrentDictionary<Type, JsonConverter> _cache = new();

   /// <inheritdoc />
   public override bool CanConvert(Type objectType)
   {
      return _cache.ContainsKey(objectType)
             || MetadataLookup.Find(objectType) is not null
             || objectType.GetCustomAttributes<ObjectFactoryAttribute>().Any(a => a.UseForSerialization.HasFlag(SerializationFrameworks.NewtonsoftJson));
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
      // type could be a derived type (like nested Smart Enum)
      var metadata = MetadataLookup.Find(type) as Metadata.Keyed;
      var modelType = metadata?.Type ?? type;

      var customFactory = modelType.GetCustomAttributes<ObjectFactoryAttribute>()
                                   .LastOrDefault(a => a.UseForSerialization.HasFlag(SerializationFrameworks.NewtonsoftJson));

      Type keyType;

      if (customFactory is not null)
      {
         keyType = customFactory.Type;
      }
      else if (metadata is not null)
      {
         keyType = metadata.KeyType;
      }
      else
      {
         throw new NotSupportedException($"The type '{type.FullName}' is not supported by the '{nameof(ThinktectureNewtonsoftJsonConverterFactory)}'.");
      }

      var validationErrorType = metadata?.ValidationErrorType ?? modelType.GetCustomAttribute<ValidationErrorAttribute>()?.Type ?? typeof(ValidationError);

      var converterType = typeof(ThinktectureNewtonsoftJsonConverter<,,>).MakeGenericType(modelType, keyType, validationErrorType);
      var converter = Activator.CreateInstance(converterType);

      return (JsonConverter)(converter ?? throw new Exception($"Could not create converter of type '{converterType.Name}'."));
   }
}

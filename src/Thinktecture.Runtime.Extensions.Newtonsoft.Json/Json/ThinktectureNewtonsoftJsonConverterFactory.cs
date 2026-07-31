using System.Collections.Concurrent;
using System.Reflection;
using Newtonsoft.Json;
using Thinktecture.Internal;

namespace Thinktecture.Json;

/// <summary>
/// Non-generic converter for Value Objects.
/// </summary>
public class ThinktectureNewtonsoftJsonConverterFactory : JsonConverter
{
   private static readonly ConcurrentDictionary<Type, JsonConverter> _cache = new();

   private readonly bool _skipObjectsWithJsonConverterAttribute;
   private readonly ConcurrentDictionary<Type, bool> _canConvertCache = new();

   /// <summary>
   /// Initializes new instance of <see cref="ThinktectureNewtonsoftJsonConverterFactory"/>.
   /// </summary>
   public ThinktectureNewtonsoftJsonConverterFactory()
      : this(true)
   {
   }

   /// <summary>
   /// Initializes new instance of <see cref="ThinktectureNewtonsoftJsonConverterFactory"/>.
   /// </summary>
   /// <param name="skipObjectsWithJsonConverterAttribute">
   /// Indication whether to skip value objects with <see cref="JsonConverterAttribute"/>.
   /// </param>
   public ThinktectureNewtonsoftJsonConverterFactory(
      bool skipObjectsWithJsonConverterAttribute)
   {
      _skipObjectsWithJsonConverterAttribute = skipObjectsWithJsonConverterAttribute;
   }

   /// <inheritdoc />
   public override bool CanConvert(Type objectType)
   {
      // Newtonsoft.Json calls CanConvert once per value, not once per type, so the result is memoized.
      // The memoization is per instance, not via the static converter cache: that cache is shared across
      // all factory instances, but "_skipObjectsWithJsonConverterAttribute" is per-instance. A converter
      // cached by another instance must not let this instance bypass its own skip-attribute policy.
      return _canConvertCache.GetOrAdd(objectType, static (type, factory) => factory.CanConvertCore(type), this);
   }

   private bool CanConvertCore(Type objectType)
   {
      var metadata = FindMetadataForConversion(objectType);

      if (metadata is null)
         return false;

      if (!_skipObjectsWithJsonConverterAttribute)
         return true;

      var jsonConverterAttribute = metadata.Value.Type.GetCustomAttribute<JsonConverterAttribute>();

      if (jsonConverterAttribute is null)
         return true;

      if (jsonConverterAttribute.ConverterType == typeof(ThinktectureNewtonsoftJsonConverterFactory))
         return true;

      return false;
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

   private static JsonConverter CreateConverter(Type typeToConvert)
   {
      // type could be a derived type (like nested Smart Enum)
      var metadata = FindMetadataForConversion(typeToConvert);

      if (metadata is null)
         throw new InvalidOperationException($"No metadata for provided type '{typeToConvert.Name}' found.");

      var converterType = typeof(ThinktectureNewtonsoftJsonConverter<,,>).MakeGenericType(metadata.Value.Type, metadata.Value.KeyType, metadata.Value.ValidationErrorType);
      var converter = Activator.CreateInstance(converterType);

      return (JsonConverter)(converter ?? throw new Exception($"Could not create converter of type '{converterType.Name}'."));
   }

   private static ConversionMetadata? FindMetadataForConversion(Type objectType)
   {
      return MetadataLookup.FindMetadataForConversion(
         objectType,
         // Object factories with a ref struct value type are excluded because a ref struct cannot be used as the
         // generic key argument of ThinktectureNewtonsoftJsonConverter. Such factories fall back to the key-based
         // metadata.
         f => !f.ValueType.IsByRefLike && f.UseForSerialization.HasSerializationFramework(SerializationFrameworks.NewtonsoftJson),
         _ => true);
   }
}

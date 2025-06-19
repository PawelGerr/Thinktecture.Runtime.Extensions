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
public class ThinktectureNewtonsoftJsonConverterFactory : JsonConverter
{
   private static readonly ConcurrentDictionary<Type, JsonConverter> _cache = new();

   private readonly bool _skipObjectsWithJsonConverterAttribute;

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
      if (_cache.ContainsKey(objectType))
         return true;

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
         f => f.UseForSerialization.HasFlag(SerializationFrameworks.NewtonsoftJson),
         _ => true);
   }
}

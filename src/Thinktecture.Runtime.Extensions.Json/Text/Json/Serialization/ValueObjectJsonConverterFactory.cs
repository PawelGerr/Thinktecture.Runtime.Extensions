using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Thinktecture.Internal;

namespace Thinktecture.Text.Json.Serialization;

/// <summary>
/// Factory for creation of <see cref="ValueObjectJsonConverter{T,TKey,TValidationError}"/>.
/// </summary>
public sealed class ValueObjectJsonConverterFactory<T, TKey, TValidationError> : JsonConverterFactory
   where T : IValueObjectFactory<T, TKey, TValidationError>, IValueObjectConvertable<TKey>
   where TKey : notnull
   where TValidationError : class, IValidationError<TValidationError>
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

      return new ValueObjectJsonConverter<T, TKey, TValidationError>(options);
   }
}

/// <summary>
/// Factory for creation of <see cref="ValueObjectJsonConverter{T,TKey,TValidationError}"/>.
/// </summary>
public sealed class ValueObjectJsonConverterFactory<T, TValidationError> : JsonConverterFactory
   where T : IValueObjectFactory<T, string, TValidationError>, IValueObjectConvertable<string>
   where TValidationError : class, IValidationError<TValidationError>
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

      return new ValueObjectJsonConverter<T, TValidationError>(options);
   }
}

/// <summary>
/// Factory for creation of <see cref="ValueObjectJsonConverter{T,TKey,TValidationError}"/>.
/// </summary>
public sealed class ValueObjectJsonConverterFactory : JsonConverterFactory
{
   /// <inheritdoc />
   public override bool CanConvert(Type typeToConvert)
   {
      return KeyedValueObjectMetadataLookup.Find(typeToConvert) is not null
             || typeToConvert.GetCustomAttributes<ValueObjectFactoryAttribute>().Any(a => a.UseForSerialization.HasFlag(SerializationFrameworks.SystemTextJson));
   }

   /// <inheritdoc />
   public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
   {
      ArgumentNullException.ThrowIfNull(typeToConvert);
      ArgumentNullException.ThrowIfNull(options);

      // typeToConvert could be derived type (like nested Smart Enum)
      var metadata = KeyedValueObjectMetadataLookup.Find(typeToConvert);
      var type = metadata?.Type ?? typeToConvert;

      var customFactory = type.GetCustomAttributes<ValueObjectFactoryAttribute>()
                              .LastOrDefault(a => a.UseForSerialization.HasFlag(SerializationFrameworks.SystemTextJson));

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
         throw new InvalidOperationException($"No metadata for provided type '{typeToConvert.Name}' found.");
      }

      var validationErrorType = type.GetCustomAttribute<ValueObjectValidationErrorAttribute>()?.Type ?? typeof(ValidationError);

      var converterType = keyType == typeof(string)
                             ? typeof(ValueObjectJsonConverter<,>).MakeGenericType(type, validationErrorType)
                             : typeof(ValueObjectJsonConverter<,,>).MakeGenericType(type, keyType, validationErrorType);
      var converter = Activator.CreateInstance(converterType, options);

      return (JsonConverter)(converter ?? throw new Exception($"Could not create converter of type '{converterType.Name}'."));
   }
}

using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Thinktecture.Internal;

namespace Thinktecture.EntityFrameworkCore.Storage.ValueConversion;

/// <summary>
/// Value converter for Smart Enums and for Value Objects with a key member.
/// </summary>
[Obsolete("Use 'AddThinktectureValueConverters()' or 'UseThinktectureValueConverters()' with corresponding builder instead.")]
public sealed class ValueObjectValueConverterFactory : ThinktectureValueConverterFactory;

/// <summary>
/// Value converter for Smart Enums and for Value Objects with a key member.
/// </summary>
[ThinktectureRuntimeExtensionInternal]
public class ThinktectureValueConverterFactory
{
   private readonly record struct CacheKey(
      Type Type,
      Type KeyType,
      Type ValidationErrorType,
      bool UseConstructorForRead);

   private static readonly ConcurrentDictionary<CacheKey, ValueConverter> _cache = new();

   /// <summary>
   /// Creates a value converter for Smart Enums and keyed Value Objects and types with object factories.
   /// </summary>
   /// <param name="useConstructorForRead">For keyed value objects only. Use the constructor instead of the factory method when reading the data from the database.</param>
   /// <typeparam name="T">Type of the value object or the Smart Enum.</typeparam>
   /// <typeparam name="TKey">Type of the key.</typeparam>
   /// <returns>An instance of <see cref="ValueConverter{T,TKey}"/>></returns>
   public static ValueConverter<T, TKey> Create<T, TKey>(bool useConstructorForRead = true)
      where T : IObjectFactory<T, TKey, ValidationError>, IConvertible<TKey>
      where TKey : notnull
   {
      return Create<T, TKey, ValidationError>(useConstructorForRead);
   }

   /// <summary>
   /// Creates a value converter for Smart Enums, keyed Value Objects and types with object factories.
   /// </summary>
   /// <param name="useConstructorForRead">For keyed value objects only. Use the constructor instead of the factory method when reading the data from the database.</param>
   /// <typeparam name="T">Type of the value object or the Smart Enum.</typeparam>
   /// <typeparam name="TKey">Type of the key.</typeparam>
   /// <typeparam name="TValidationError">Type of the validation error.</typeparam>
   /// <returns>An instance of <see cref="ValueConverter{T,TKey}"/>></returns>
   public static ValueConverter<T, TKey> Create<T, TKey, TValidationError>(bool useConstructorForRead = true)
      where T : IObjectFactory<T, TKey, TValidationError>, IConvertible<TKey>
      where TKey : notnull
      where TValidationError : class, IValidationError<TValidationError>
   {
      return (ValueConverter<T, TKey>)_cache.GetOrAdd(
         new CacheKey(typeof(T), typeof(TKey), typeof(TValidationError), useConstructorForRead),
         static cacheKey => new ThinktectureValueConverter<T, TKey, TValidationError>(cacheKey.UseConstructorForRead));
   }

   /// <summary>
   /// Creates a value converter for Smart Enums and for keyed Value Objects.
   /// </summary>
   /// <param name="type">Type of the value object or the Smart Enum.</param>
   /// <param name="useConstructorForRead">For keyed value objects only. Use the constructor instead of the factory method when reading the data from the database.</param>
   /// <returns>An instance of <see cref="ValueConverter"/>></returns>
   public static ValueConverter Create(
      Type type,
      bool useConstructorForRead = true)
   {
      ArgumentNullException.ThrowIfNull(type);

      var metadata = MetadataLookup.FindMetadataForConversion(
         type,
         f => f.UseWithEntityFramework,
         _ => true);

      if (metadata is null)
         throw new NotSupportedException($"The provided type '{type.FullName}' is not supported by {nameof(ThinktectureValueConverterFactory)}.");

      return Create(metadata.Value, useConstructorForRead);
   }

   internal static ValueConverter Create(
      ConversionMetadata metadata,
      bool useConstructorForRead)
   {
      return _cache.GetOrAdd(new CacheKey(metadata.Type, metadata.KeyType, metadata.ValidationErrorType, useConstructorForRead),
                             static (cacheKey, metadata) =>
                             {
                                var converterType = typeof(ThinktectureValueConverter<,,>).MakeGenericType(cacheKey.Type, cacheKey.KeyType, cacheKey.ValidationErrorType);
                                var converter = Activator.CreateInstance(converterType, metadata, cacheKey.UseConstructorForRead)
                                                ?? throw new Exception($"Could not create an instance of '{converterType.Name}'.");

                                return (ValueConverter)converter;
                             }, metadata);
   }

   private sealed class ThinktectureValueConverter<T, TProvider, TValidationError> : ValueConverter<T, TProvider>
      where T : IObjectFactory<T, TProvider, TValidationError>, IConvertible<TProvider>
      where TProvider : notnull
      where TValidationError : class, IValidationError<TValidationError>
   {
      public override Func<object?, object?> ConvertToProvider { get; }

      public ThinktectureValueConverter(bool useConstructorIfExists)
         : this(GetConverterFromProvider(useConstructorIfExists))
      {
      }

      public ThinktectureValueConverter(ConversionMetadata conversionMetadata, bool useConstructorIfExists)
         : this(GetConverterFromProvider(conversionMetadata, useConstructorIfExists))
      {
      }

      public ThinktectureValueConverter(Expression<Func<TProvider, T>> convertFromProvider)
         : base(static o => o.ToValue(), convertFromProvider)
      {
         ConvertToProvider = o => o switch
         {
            null => null,
            TProvider key => key, // useful for comparisons of value objects with their key types in LINQ queries
            T obj => obj.ToValue(),
            _ => Convert(o)
         };
      }

      private static Expression<Func<TProvider, T>> GetConverterFromProvider(bool useConstructorIfExists)
      {
         var metadata = MetadataLookup.FindMetadataForConversion(
            typeof(T),
            f => f.ValueType == typeof(TProvider),
            m => m.KeyType == typeof(TProvider));

         return GetConverterFromProvider(metadata, useConstructorIfExists);
      }

      private static Expression<Func<TProvider, T>> GetConverterFromProvider(
         ConversionMetadata? metadata,
         bool useConstructorIfExists)
      {
         if (metadata is not null)
         {
            if (useConstructorIfExists && metadata.Value.ConvertFromKeyExpressionViaConstructor is not null)
               return (Expression<Func<TProvider, T>>)metadata.Value.ConvertFromKeyExpressionViaConstructor;

            if (metadata.Value.ConvertFromKeyExpression is not null)
               return (Expression<Func<TProvider, T>>)metadata.Value.ConvertFromKeyExpression;
         }

         return value => CreateFromProvider(value);
      }

      private static T CreateFromProvider(TProvider value)
      {
         var validationError = T.Validate(value, null, out var obj);

         return validationError is null
                   ? obj ?? throw new Exception($"ObjectFactory<{typeof(TProvider).FullName}> with '{nameof(ObjectFactoryAttribute.UseWithEntityFramework)} = true' on type {typeof(T).FullName} must not return null for non-null values.")
                   : throw new ValidationException(validationError.ToString());
      }

      private static TProvider Convert(object value)
      {
         return (TProvider)System.Convert.ChangeType(value, typeof(TProvider));
      }
   }
}

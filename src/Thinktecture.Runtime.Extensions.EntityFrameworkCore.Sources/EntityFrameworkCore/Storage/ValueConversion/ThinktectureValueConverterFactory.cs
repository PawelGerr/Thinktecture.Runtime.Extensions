using System.Collections.Concurrent;
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
      bool UseConstructorForRead);

   private static readonly ConcurrentDictionary<CacheKey, ValueConverter> _cache = new();

   /// <summary>
   /// Creates a value converter for Smart Enums and for keyed Value Objects.
   /// </summary>
   /// <param name="useConstructorForRead">For keyed value objects only. Use the constructor instead of the factory method when reading the data from the database.</param>
   /// <typeparam name="T">Type of the value object or the Smart Enum.</typeparam>
   /// <typeparam name="TKey">Type of the key.</typeparam>
   /// <returns>An instance of <see cref="ValueConverter{T,TKey}"/>></returns>
   public static ValueConverter<T, TKey> Create<T, TKey>(bool useConstructorForRead = true)
      where T : IObjectFactory<TKey>, IConvertible<TKey>
      where TKey : notnull
   {
      return new ThinktectureValueConverter<T, TKey>(useConstructorForRead);
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

      if (MetadataLookup.Find(type) is not Metadata.Keyed metadata)
         throw new NotSupportedException($"The provided type '{type.FullName}' is not supported.");

      return Create(metadata, useConstructorForRead);
   }

   internal static ValueConverter Create(
      Metadata.Keyed metadata,
      bool useConstructorForRead)
   {
      return _cache.GetOrAdd(new CacheKey(metadata.Type, metadata.KeyType, useConstructorForRead),
                             static cacheKey =>
                             {
                                var converterType = typeof(ThinktectureValueConverter<,>).MakeGenericType(cacheKey.Type, cacheKey.KeyType);
                                var converter = Activator.CreateInstance(converterType, [cacheKey.UseConstructorForRead]) ?? throw new Exception($"Could not create an instance of '{converterType.Name}'.");

                                return (ValueConverter)converter;
                             });
   }

   private static Expression<Func<TKey, T>> GetConverterFromKey<T, TKey>(bool useConstructor)
      where T : IObjectFactory<TKey>, IConvertible<TKey>
      where TKey : notnull
   {
      var metadata = MetadataLookup.Find(typeof(T));

      if (metadata is null)
         throw new InvalidOperationException($"No metadata for provided type '{typeof(T).Name}' found.");

      var factoryMethod = metadata.Switch(
         keyedSmartEnum: m => m.ConvertFromKeyExpression,
         keyedValueObject: m => useConstructor
                                   ? m.ConvertFromKeyExpressionViaConstructor
                                   : m.ConvertFromKeyExpression ?? m.ConvertFromKeyExpressionViaConstructor,
         complexValueObject: m => throw new NotSupportedException($"Complex value objects are not supported. Type: '{m.Type.FullName}'."),
         adHocUnion: m => throw new NotSupportedException($"Ad hoc unions are not supported. Type: '{m.Type.FullName}'."));

      return (Expression<Func<TKey, T>>)factoryMethod;
   }

   private static T Convert<T>(object value)
   {
      return (T)System.Convert.ChangeType(value, typeof(T));
   }

   private sealed class ThinktectureValueConverter<T, TKey> : ValueConverter<T, TKey>
      where T : IObjectFactory<TKey>, IConvertible<TKey>
      where TKey : notnull
   {
      /// <inheritdoc />
      public override Func<object?, object?> ConvertToProvider { get; }

      public ThinktectureValueConverter(bool useConstructor)
         : base(static o => o.ToValue(), GetConverterFromKey<T, TKey>(useConstructor))
      {
         ConvertToProvider = o => o switch
         {
            null => null,
            TKey key => key, // useful for comparisons of value objects with its key types in LINQ queries
            T obj => obj.ToValue(),
            _ => Convert<TKey>(o)
         };
      }
   }
}

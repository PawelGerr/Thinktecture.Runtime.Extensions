using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Thinktecture.Internal;

namespace Thinktecture.EntityFrameworkCore.Storage.ValueConversion;

/// <summary>
/// Value converter for Value Objects and <see cref="IKeyedValueObject{TKey}"/>.
/// </summary>
public sealed class ValueObjectValueConverterFactory
{
   /// <summary>
   /// Creates a value converter for value objects with a key property and <see cref="IKeyedValueObject{TKey}"/>.
   /// </summary>
   /// <param name="useConstructorForRead">For keyed value objects only. Use the constructor instead of the factory method when reading the data from database.</param>
   /// <typeparam name="T">Type of the value object.</typeparam>
   /// <typeparam name="TKey">Type of the key.</typeparam>
   /// <returns>An instance of <see cref="ValueConverter{T,TKey}"/>></returns>
   public static ValueConverter<T, TKey> Create<T, TKey>(bool useConstructorForRead = true)
      where T : IValueObjectFactory<TKey>, IValueObjectConverter<TKey>
      where TKey : notnull
   {
      return new ValueObjectValueConverter<T, TKey>(useConstructorForRead);
   }

   /// <summary>
   /// Creates a value converter for a validatable Smart Enum.
   /// </summary>
   /// <param name="validateOnWrite">Ensures that the item is valid before writing it to database.</param>
   /// <typeparam name="TEnum">Type of the enum.</typeparam>
   /// <typeparam name="TKey">Type of the key.</typeparam>
   /// <returns>An instance of <see cref="ValueConverter{TEnum,TKey}"/>></returns>
   public static ValueConverter<TEnum, TKey> CreateForValidatableEnum<TEnum, TKey>(bool validateOnWrite)
      where TEnum : IValidatableEnum, IValueObjectFactory<TKey>, IValueObjectConverter<TKey>
      where TKey : notnull
   {
      return new ValidatableEnumValueConverter<TEnum, TKey>(validateOnWrite);
   }

   /// <summary>
   /// Creates a value converter for validatable Smart Enum.
   /// </summary>
   /// <param name="type">Type of the value object/enum.</param>
   /// <param name="validateOnWrite">In case of a validatable Smart Enum, ensures that the item is valid before writing it to database.</param>
   /// <param name="useConstructorForRead">For keyed value objects only. Use the constructor instead of the factory method when reading the data from database.</param>
   /// <returns>An instance of <see cref="ValueConverter"/>></returns>
   public static ValueConverter Create(
      Type type,
      bool validateOnWrite,
      bool useConstructorForRead = true)
   {
      ArgumentNullException.ThrowIfNull(type);

      var metadata = KeyedValueObjectMetadataLookup.Find(type);

      if (metadata is null)
         throw new ArgumentException($"The provided type '{type.Name}' is neither an Smart Enum nor a Value Object with a key member.");

      return Create(metadata, validateOnWrite, useConstructorForRead);
   }

   internal static ValueConverter Create(
      KeyedValueObjectMetadata metadata,
      bool validateOnWrite,
      bool useConstructorForRead)
   {
      object converter;

      if (metadata.IsValidatableEnum)
      {
         var enumConverterType = typeof(ValidatableEnumValueConverter<,>).MakeGenericType(metadata.Type, metadata.KeyType);
         converter = Activator.CreateInstance(enumConverterType, new object[] { validateOnWrite }) ?? throw new Exception($"Could not create an instance of '{enumConverterType.Name}'.");
      }
      else
      {
         var converterType = typeof(ValueObjectValueConverter<,>).MakeGenericType(metadata.Type, metadata.KeyType);
         converter = Activator.CreateInstance(converterType, new object[] { useConstructorForRead }) ?? throw new Exception($"Could not create an instance of '{converterType.Name}'.");
      }

      return (ValueConverter)converter;
   }

   private static Expression<Func<TKey, T>> GetConverterFromKey<T, TKey>(bool useConstructor)
      where T : IValueObjectFactory<TKey>, IValueObjectConverter<TKey>
      where TKey : notnull
   {
      var metadata = KeyedValueObjectMetadataLookup.Find(typeof(T));

      if (metadata is null)
         throw new InvalidOperationException($"No metadata for provided type '{typeof(T).Name}' found.");

      var factoryMethod = useConstructor
                             ? metadata.ConvertFromKeyExpressionViaConstructor ?? metadata.ConvertFromKeyExpression
                             : metadata.ConvertFromKeyExpression;

      return (Expression<Func<TKey, T>>)(factoryMethod ?? throw new InvalidOperationException($"A value converter cannot be created for the type '{typeof(T).Name}' because it has no factory methods."));
   }

   private sealed class ValueObjectValueConverter<T, TKey> : ValueConverter<T, TKey>
      where T : IValueObjectFactory<TKey>, IValueObjectConverter<TKey>
      where TKey : notnull
   {
      public ValueObjectValueConverter(bool useConstructor)
         : base(static o => o.ToValue(), GetConverterFromKey<T, TKey>(useConstructor))
      {
      }
   }

   private sealed class ValidatableEnumValueConverter<TEnum, TKey> : ValueConverter<TEnum, TKey>
      where TEnum : IValidatableEnum, IValueObjectFactory<TKey>, IValueObjectConverter<TKey>
      where TKey : notnull
   {
      public ValidatableEnumValueConverter(bool validateOnWrite)
         : base(GetKeyProvider(validateOnWrite), GetConverterFromKey<TEnum, TKey>(false))
      {
      }

      private static Expression<Func<TEnum, TKey>> GetKeyProvider(bool validateOnWrite)
      {
         return validateOnWrite
                   ? static item => GetKeyIfValid(item)
                   : static item => item.ToValue();
      }

      private static TKey GetKeyIfValid(TEnum item)
      {
         item.EnsureValid();
         return item.ToValue();
      }
   }
}

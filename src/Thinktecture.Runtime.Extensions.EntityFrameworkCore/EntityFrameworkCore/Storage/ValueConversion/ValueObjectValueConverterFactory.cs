using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Thinktecture.Internal;

namespace Thinktecture.EntityFrameworkCore.Storage.ValueConversion;

/// <summary>
/// Value converter for value objects and <see cref="IEnum{TKey}"/>.
/// </summary>
public class ValueObjectValueConverterFactory
{
   /// <summary>
   /// Creates a value converter for value objects with a key property and <see cref="IEnum{TKey}"/>.
   /// </summary>
   /// <typeparam name="T">Type of the value object.</typeparam>
   /// <typeparam name="TKey">Type of the key.</typeparam>
   /// <returns>An instance of <see cref="ValueConverter{T,TKey}"/>></returns>
   public static ValueConverter<T, TKey> Create<T, TKey>()
      where TKey : notnull
   {
      return new ValueObjectValueConverter<T, TKey>();
   }

   /// <summary>
   /// Creates a value converter for <see cref="IValidatableEnum{TKey}"/>.
   /// </summary>
   /// <param name="validateOnWrite">Ensures that the item is valid before writing it to database.</param>
   /// <typeparam name="TEnum">Type of the enum.</typeparam>
   /// <typeparam name="TKey">Type of the key.</typeparam>
   /// <returns>An instance of <see cref="ValueConverter{TEnum,TKey}"/>></returns>
   public static ValueConverter<TEnum, TKey> Create<TEnum, TKey>(bool validateOnWrite)
      where TEnum : IValidatableEnum<TKey>
      where TKey : notnull
   {
      return new ValidatableEnumValueConverter<TEnum, TKey>(validateOnWrite);
   }

   /// <summary>
   /// Creates a value converter for <see cref="IValidatableEnum{TKey}"/>.
   /// </summary>
   /// <param name="type">Type of the value object/enum.</param>
   /// <param name="validateOnWrite">In case of an <see cref="IValidatableEnum{TKey}"/>, ensures that the item is valid before writing it to database.</param>
   /// <returns>An instance of <see cref="ValueConverter"/>></returns>
   public static ValueConverter Create(Type type, bool validateOnWrite)
   {
      if (type is null)
         throw new ArgumentNullException(nameof(type));

      var metadata = ValueObjectMetadataLookup.Find(type);

      if (metadata is null)
         throw new ArgumentException($"The provided type '{type.Name}' is neither an enumeration nor a value object with a key member.");

      object converter;

      if (metadata.IsValidatableEnum)
      {
         var enumConverterType = typeof(ValidatableEnumValueConverter<,>).MakeGenericType(metadata.Type, metadata.KeyType);
         converter = Activator.CreateInstance(enumConverterType, new object[] { validateOnWrite }) ?? throw new Exception($"Could not create an instance of '{enumConverterType.Name}'.");
      }
      else
      {
         var converterType = typeof(ValueObjectValueConverter<,>).MakeGenericType(metadata.Type, metadata.KeyType);
         converter = Activator.CreateInstance(converterType) ?? throw new Exception($"Could not create an instance of '{converterType.Name}'.");
      }

      return (ValueConverter)converter;
   }

   private static Expression<Func<TKey, T>> GetConverterFromKey<T, TKey>()
   {
      var metadata = ValueObjectMetadataLookup.Find(typeof(T));

      if (metadata is null)
         throw new InvalidOperationException($"No metadata for provided type '{typeof(T).Name}' found.");

      return (Expression<Func<TKey, T>>)metadata.ConvertFromKeyExpression;
   }

   private static Expression<Func<T, TKey>> GetConverterToKey<T, TKey>()
   {
      var metadata = ValueObjectMetadataLookup.Find(typeof(T));

      if (metadata is null)
         throw new InvalidOperationException($"No metadata for provided type '{typeof(T).Name}' found.");

      return (Expression<Func<T, TKey>>)metadata.ConvertToKeyExpression;
   }

   private class ValueObjectValueConverter<T, TKey> : ValueConverter<T, TKey>
      where TKey : notnull
   {
      public ValueObjectValueConverter()
         : base(GetConverterToKey<T, TKey>(), GetConverterFromKey<T, TKey>())
      {
      }
   }

   private class ValidatableEnumValueConverter<TEnum, TKey> : ValueConverter<TEnum, TKey>
      where TEnum : IValidatableEnum<TKey>
      where TKey : notnull
   {
      public ValidatableEnumValueConverter(bool validateOnWrite)
         : base(GetKeyProvider(validateOnWrite), GetConverterFromKey<TEnum, TKey>())
      {
      }

      private static Expression<Func<TEnum, TKey>> GetKeyProvider(bool validateOnWrite)
      {
         if (validateOnWrite)
            return item => GetKeyIfValid(item);

         return item => item.GetKey();
      }

      private static TKey GetKeyIfValid(TEnum item)
      {
         item.EnsureValid();
         return item.GetKey();
      }
   }
}
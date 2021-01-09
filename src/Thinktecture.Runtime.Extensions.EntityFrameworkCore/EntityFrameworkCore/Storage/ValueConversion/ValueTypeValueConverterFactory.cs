using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Thinktecture.EntityFrameworkCore.Storage.ValueConversion
{
   /// <summary>
   /// Value converter for value types and <see cref="IEnum{TKey}"/>.
   /// </summary>
   public class ValueTypeValueConverterFactory
   {
      /// <summary>
      /// Creates a value converter for value types with a key property and <see cref="IEnum{TKey}"/>.
      /// </summary>
      /// <typeparam name="T">Type of the value type.</typeparam>
      /// <typeparam name="TKey">Type of the key.</typeparam>
      /// <returns>An instance of <see cref="ValueConverter{T,TKey}"/>></returns>
      public static ValueConverter<T, TKey> Create<T, TKey>()
         where TKey : notnull
      {
         return new ValueTypeValueConverter<T, TKey>();
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

      private static Expression<Func<TKey, T>> GetConverterFromKey<T, TKey>()
      {
         var metadata = ValueTypeMetadataLookup.Find(typeof(T));

         if (metadata is null)
            throw new InvalidOperationException($"No metadata for provided type '{typeof(T).Name}' found.");

         return (Expression<Func<TKey, T>>)metadata.ConvertFromKeyExpression;
      }

      private static Expression<Func<T, TKey>> GetConverterToKey<T, TKey>()
      {
         var metadata = ValueTypeMetadataLookup.Find(typeof(T));

         if (metadata is null)
            throw new InvalidOperationException($"No metadata for provided type '{typeof(T).Name}' found.");

         return (Expression<Func<T, TKey>>)metadata.ConvertToKeyExpression;
      }

      private class ValueTypeValueConverter<T, TKey> : ValueConverter<T, TKey>
         where TKey : notnull
      {
         public ValueTypeValueConverter()
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
}

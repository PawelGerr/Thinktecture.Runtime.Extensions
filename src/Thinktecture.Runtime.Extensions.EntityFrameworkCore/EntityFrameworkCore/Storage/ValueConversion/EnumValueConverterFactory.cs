using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Thinktecture.EntityFrameworkCore.Storage.ValueConversion
{
   /// <summary>
   /// Value converter for <see cref="IEnum{TKey}"/>.
   /// </summary>
   public class EnumValueConverterFactory
   {
      /// <summary>
      /// Creates a value converter for <see cref="IEnum{TKey}"/>.
      /// </summary>
      /// <typeparam name="TEnum">Type of the enum.</typeparam>
      /// <typeparam name="TKey">Type of the key.</typeparam>
      /// <returns>An instance of <see cref="ValueConverter{TEnum,TKey}"/>></returns>
      public static ValueConverter<TEnum, TKey> Create<TEnum, TKey>()
         where TEnum : IEnum<TKey>
         where TKey : notnull
      {
         return new NonValidatableEnumValueConverter<TEnum, TKey>();
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

      private static Expression<Func<TKey, TEnum>> GetConverter<TEnum, TKey>()
      {
         var enumMetadata = EnumMetadataLookup.FindEnum(typeof(TEnum));

         if (enumMetadata is null)
            throw new InvalidOperationException($"No metadata for provided type '{typeof(TEnum).Name}' found.");

         return (Expression<Func<TKey, TEnum>>)enumMetadata.ConvertFromKeyExpression;
      }

      private class NonValidatableEnumValueConverter<TEnum, TKey> : ValueConverter<TEnum, TKey>
         where TEnum : IEnum<TKey>
         where TKey : notnull
      {
         public NonValidatableEnumValueConverter()
            : base(item => item.GetKey(), GetConverter<TEnum, TKey>())
         {
         }
      }

      private class ValidatableEnumValueConverter<TEnum, TKey> : ValueConverter<TEnum, TKey>
         where TEnum : IValidatableEnum<TKey>
         where TKey : notnull
      {
         public ValidatableEnumValueConverter(bool validateOnWrite)
            : base(GetKeyProvider(validateOnWrite), GetConverter<TEnum, TKey>())
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

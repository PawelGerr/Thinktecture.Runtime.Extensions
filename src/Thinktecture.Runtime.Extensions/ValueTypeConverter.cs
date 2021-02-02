using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

namespace Thinktecture
{
   /// <summary>
   /// Type converter to convert an <see cref="IEnum{TKey}"/> to <typeparamref name="TKey"/> and vice versa.
   /// </summary>
   /// <typeparam name="T">Type of the concrete enumeration.</typeparam>
   /// <typeparam name="TKey">Type of the key.</typeparam>
   public abstract class ValueTypeConverter<T, TKey> : TypeConverter
      where TKey : notnull
   {
      private static readonly Type _type = typeof(T);
      private static readonly Type? _nullableType = !typeof(T).IsClass ? typeof(Nullable<>).MakeGenericType(typeof(T)) : null;
      private static readonly Type _keyType = typeof(TKey);
      private static readonly Type? _nullableKeyType = !typeof(TKey).IsClass ? typeof(Nullable<>).MakeGenericType(typeof(TKey)) : null;
      private static readonly TypeConverter? _keyConverter = typeof(TKey) != typeof(T) ? TypeDescriptor.GetConverter(typeof(TKey)) : null;

      /// <summary>
      /// Converts <paramref name="key"/> to an instance of <typeparamref name="T"/>.
      /// </summary>
      /// <param name="key">Key to convert.</param>
      /// <returns>An instance of <typeparamref name="T"/>.</returns>
      [return: NotNullIfNotNull("key")]
      protected abstract T? ConvertFrom(TKey? key);

      /// <summary>
      /// Converts <paramref name="obj"/> to an instance of <typeparamref name="TKey"/>.
      /// </summary>
      /// <param name="obj">Object to convert.</param>
      /// <returns>An instance of <typeparamref name="TKey"/>.</returns>
      protected abstract TKey GetKeyValue(T obj);

      /// <inheritdoc />
      public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
      {
         if (sourceType == _keyType || sourceType == _nullableKeyType || sourceType == _type || sourceType == _nullableType)
            return true;

         return _keyConverter?.CanConvertFrom(context, sourceType)
                ?? base.CanConvertFrom(context, sourceType);
      }

      /// <inheritdoc />
      public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
      {
         if (destinationType == _keyType || destinationType == _nullableKeyType || destinationType == _type || destinationType == _nullableType)
            return true;

         return _keyConverter?.CanConvertTo(context, destinationType)
                ?? base.CanConvertTo(context, destinationType);
      }

      /// <inheritdoc />
      public override object? ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object? value)
      {
         if (value is null)
         {
            if (_type.IsClass)
               return null;

            throw new NotSupportedException($"{_type.Name} is a struct and cannot be converted from 'null'.");
         }

         if (value is T obj)
            return obj;

         if (value is TKey key)
            return ConvertFrom(key);

         if (_keyConverter is not null)
         {
            key = (TKey?)_keyConverter.ConvertFrom(context, culture, value);

            return ConvertFrom(key);
         }

         return base.ConvertFrom(context, culture, value);
      }

      /// <inheritdoc />
      public override object? ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object? value, Type destinationType)
      {
         if (value is null)
         {
            if (destinationType.IsClass || Nullable.GetUnderlyingType(destinationType) is not null)
               return null;

            throw new NotSupportedException($"{destinationType.Name} is a struct and cannot be converted to 'null'.");
         }

         if (value is T item)
         {
            var underlyingType = Nullable.GetUnderlyingType(destinationType);

            if (destinationType == _keyType || underlyingType == _keyType)
               return GetKeyValue(item);

            if (destinationType == _type || underlyingType == _type)
               return value;

            if (_keyConverter is not null)
               return _keyConverter.ConvertTo(context, culture, GetKeyValue(item), destinationType);
         }

         return base.ConvertTo(context, culture, value, destinationType);
      }
   }
}

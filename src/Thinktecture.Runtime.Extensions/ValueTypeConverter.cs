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
      private static readonly Type _keyType = typeof(TKey);

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
         if (sourceType == typeof(TKey) || sourceType == typeof(T))
            return true;

         if (typeof(TKey) != typeof(T))
            return GetKeyConverter().CanConvertFrom(context, sourceType);

         return base.CanConvertFrom(context, sourceType);
      }

      /// <inheritdoc />
      public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
      {
         if (destinationType == _keyType || destinationType == _type)
            return true;

         var underlyingType = Nullable.GetUnderlyingType(destinationType);

         if (underlyingType == _keyType || underlyingType == _type)
            return true;

         if (_keyType != _type)
            return GetKeyConverter().CanConvertTo(context, destinationType);

         return base.CanConvertTo(context, destinationType);
      }

      /// <inheritdoc />
      public override object? ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object? value)
      {
         if (value is null)
         {
            if (!_type.IsClass)
               throw new NotSupportedException($"{GetType().Name} cannot convert from 'null'.");

            return default(T);
         }

         if (value is T obj)
            return obj;

         if (value is TKey key)
            return ConvertFrom(key);

         if (_keyType != _type)
         {
            key = (TKey?)GetKeyConverter().ConvertFrom(context, culture, value);

            return ConvertFrom(key);
         }

         return base.ConvertFrom(context, culture, value);
      }

      /// <inheritdoc />
      public override object? ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object? value, Type destinationType)
      {
         if (value is null)
            return destinationType.GetTypeInfo().IsValueType ? Activator.CreateInstance(destinationType) : null;

         if (value is T item)
         {
            var underlyingType = Nullable.GetUnderlyingType(destinationType);

            if (destinationType == _keyType || underlyingType == _keyType)
               return GetKeyValue(item);

            if (destinationType == _type || underlyingType == _type)
               return value;

            if (_keyType != _type)
               return GetKeyConverter().ConvertTo(context, culture, GetKeyValue(item), destinationType);
         }

         return base.ConvertTo(context, culture, value, destinationType);
      }

      private static TypeConverter GetKeyConverter()
      {
         return TypeDescriptor.GetConverter(_keyType);
      }
   }
}

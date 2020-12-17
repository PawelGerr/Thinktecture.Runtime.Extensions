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
   /// <typeparam name="TEnum">Type of the concrete enumeration.</typeparam>
   /// <typeparam name="TKey">Type of the key.</typeparam>
   public abstract class EnumTypeConverter<TEnum, TKey> : TypeConverter
      where TEnum : IEnum<TKey>
      where TKey : notnull
   {
      /// <summary>
      /// Converts <paramref name="key"/> to an instance of <typeparamref name="TEnum"/>.
      /// </summary>
      /// <param name="key">Key to convert.</param>
      /// <returns>An instance of <typeparamref name="TEnum"/>.</returns>
      [return: NotNullIfNotNull("key")]
      protected abstract TEnum? ConvertFrom(TKey? key);

      /// <inheritdoc />
      public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
      {
         if (sourceType == typeof(TKey) || sourceType == typeof(TEnum))
            return true;

         if (typeof(TKey) != typeof(TEnum))
         {
            var keyConverter = TypeDescriptor.GetConverter(typeof(TKey));
            return keyConverter.CanConvertFrom(context, sourceType);
         }

         return base.CanConvertFrom(context, sourceType);
      }

      /// <inheritdoc />
      public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
      {
         if (destinationType == typeof(TKey) || destinationType == typeof(TEnum))
            return true;

         if (typeof(TKey) != typeof(TEnum))
         {
            var keyConverter = TypeDescriptor.GetConverter(typeof(TKey));
            return keyConverter.CanConvertTo(context, destinationType);
         }

         return base.CanConvertTo(context, destinationType);
      }

      /// <inheritdoc />
      public override object? ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object? value)
      {
         if (value is null)
            return default(TEnum);

         if (value is TKey key)
            return ConvertFrom(key);

         if (value is TEnum item)
            return item;

         if (typeof(TKey) != typeof(TEnum))
         {
            var keyConverter = TypeDescriptor.GetConverter(typeof(TKey));
            key = (TKey?)keyConverter.ConvertFrom(context, culture, value);

            return ConvertFrom(key);
         }

         return base.ConvertFrom(context, culture, value);
      }

      /// <inheritdoc />
      public override object? ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object? value, Type destinationType)
      {
         if (value is null)
            return destinationType.GetTypeInfo().IsValueType ? Activator.CreateInstance(destinationType) : null;

         if (value is TEnum item)
         {
            if (destinationType == typeof(TKey))
               return item.GetKey();
            if (destinationType == typeof(TEnum))
               return value;

            if (typeof(TKey) != typeof(TEnum))
            {
               var keyConverter = TypeDescriptor.GetConverter(typeof(TKey));
               return keyConverter.ConvertTo(context, culture, item.GetKey(), destinationType);
            }
         }

         return base.ConvertTo(context, culture, value, destinationType);
      }
   }
}

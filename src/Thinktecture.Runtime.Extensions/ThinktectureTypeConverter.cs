using System.ComponentModel;
using System.Globalization;

namespace Thinktecture;

/// <summary>
/// Type converter to convert corresponding types to <typeparamref name="TKey"/> and vice versa.
/// </summary>
/// <typeparam name="T">Type of the concrete enumeration.</typeparam>
/// <typeparam name="TKey">Type of the key.</typeparam>
/// <typeparam name="TValidationError">Type of the validation error.</typeparam>
[Obsolete("Use 'ThinktectureTypeConverter' instead. This class will be removed in a future version.")]
public class ValueObjectTypeConverter<T, TKey, TValidationError> : ThinktectureTypeConverter<T, TKey, TValidationError>
   where T : IObjectFactory<T, TKey, TValidationError>, IConvertible<TKey>
   where TKey : notnull
   where TValidationError : class, IValidationError<TValidationError>;

/// <summary>
/// Type converter to convert corresponding types to <typeparamref name="TKey"/> and vice versa.
/// </summary>
/// <typeparam name="T">Type of the concrete enumeration.</typeparam>
/// <typeparam name="TKey">Type of the key.</typeparam>
/// <typeparam name="TValidationError">Type of the validation error.</typeparam>
[ThinktectureRuntimeExtensionInternal]
public class ThinktectureTypeConverter<T, TKey, TValidationError> : TypeConverter
   where T : IObjectFactory<T, TKey, TValidationError>, IConvertible<TKey>
   where TKey : notnull
   where TValidationError : class, IValidationError<TValidationError>
{
   private static readonly Type _type = typeof(T);
   private static readonly Type? _nullableType = !typeof(T).IsClass ? typeof(Nullable<>).MakeGenericType(typeof(T)) : null;
   private static readonly Type _keyType = typeof(TKey);
   private static readonly Type? _nullableKeyType = !typeof(TKey).IsClass ? typeof(Nullable<>).MakeGenericType(typeof(TKey)) : null;
   private static readonly TypeConverter? _keyConverter = typeof(TKey) != typeof(T) ? TypeDescriptor.GetConverter(typeof(TKey)) : null;

   /// <inheritdoc />
   public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
   {
      if (sourceType == _keyType || sourceType == _nullableKeyType || sourceType == _type || sourceType == _nullableType)
         return true;

      return _keyConverter?.CanConvertFrom(context, sourceType)
             ?? base.CanConvertFrom(context, sourceType);
   }

   /// <inheritdoc />
   public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
   {
      if (destinationType == _keyType || destinationType == _nullableKeyType || destinationType == _type || destinationType == _nullableType)
         return true;

      return _keyConverter?.CanConvertTo(context, destinationType)
             ?? base.CanConvertTo(context, destinationType);
   }

   /// <inheritdoc />
   public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object? value)
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
         return ConvertFromKey(key, culture);

      if (_keyConverter is not null)
      {
         var convertedValue = _keyConverter.ConvertFrom(context, culture, value);

         if (convertedValue is TKey convertedKey)
            return ConvertFromKey(convertedKey, culture);
      }

      return base.ConvertFrom(context, culture, value);
   }

   private static T? ConvertFromKey(TKey key, IFormatProvider? culture)
   {
      var validationError = T.Validate(key, culture, out var item);

      if (validationError is null)
         return item;

      throw new FormatException(validationError.ToString());
   }

   /// <inheritdoc />
   public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
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
            return item.ToValue();

         if (destinationType == _type || underlyingType == _type)
            return value;

         if (_keyConverter is not null)
            return _keyConverter.ConvertTo(context, culture, item.ToValue(), destinationType);
      }

      return base.ConvertTo(context, culture, value, destinationType);
   }
}

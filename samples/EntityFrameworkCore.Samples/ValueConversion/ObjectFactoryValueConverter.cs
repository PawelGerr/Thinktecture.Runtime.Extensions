using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Thinktecture.ValueConversion;

public class ObjectFactoryValueConverter<T, TValue> : ObjectFactoryValueConverter<T, TValue, ValidationError>
   where T : IObjectFactory<T, TValue, ValidationError>, IConvertible<TValue>
   where TValue : notnull;

public class ObjectFactoryValueConverter<T, TValue, TValidationError> : ValueConverter<T, TValue>
   where T : IObjectFactory<T, TValue, TValidationError>, IConvertible<TValue>
   where TValidationError : class, IValidationError<TValidationError>
   where TValue : notnull
{
   public ObjectFactoryValueConverter(IFormatProvider? formatProvider = null)
      : base(o => o.ToValue(), v => CreateFromValue(formatProvider, v))
   {
   }

   private static T CreateFromValue(IFormatProvider? formatProvider, TValue value)
   {
      var error = T.Validate(value, formatProvider, out var obj);

      return error is null
                ? obj ?? throw new Exception($"ObjectFactory must not return null for not-null value '{value}'.")
                : throw new ValidationException(error.ToString());
   }
}

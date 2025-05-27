using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Thinktecture.Validation;

public class MaybeBound<T, TKey, TValidationError> : IMaybeBound, IValidatableObject // IValidatableObject for .NET 10
   where T : IObjectFactory<T, TKey, TValidationError>
   where TKey : IParsable<TKey>
   where TValidationError : class, IValidationError<TValidationError>
{
   private readonly T? _value;
   public T? Value => Error is null ? _value : throw new ValidationException(Error);

   public string? Error { get; }

   private MaybeBound(T? value, string? error)
   {
      _value = value;
      Error = error;
   }

   public static bool TryParse(string s, IFormatProvider? formatProvider, out MaybeBound<T, TKey, TValidationError> value)
   {
      if (!TKey.TryParse(s, formatProvider, out var key))
      {
         value = new MaybeBound<T, TKey, TValidationError>(default, $"The value '{s}' cannot be converted to '{typeof(TKey).FullName}'.");
      }
      else
      {
         var validationError = T.Validate(key, formatProvider, out var obj);

         value = validationError is null
                    ? new(obj, null)
                    : new(default, validationError.ToString() ?? "Invalid format");
      }

      return true;
   }

   public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
   {
      return Error is null ? [] : [new ValidationResult(Error, [validationContext.MemberName ?? nameof(Value)])];
   }
}

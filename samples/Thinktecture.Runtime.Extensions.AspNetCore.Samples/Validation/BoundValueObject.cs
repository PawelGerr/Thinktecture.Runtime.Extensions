using System;
using System.ComponentModel.DataAnnotations;

namespace Thinktecture.Validation;

public class BoundValueObject<T, TKey, TValidationError> : IBoundParam
   where T : IValueObjectFactory<T, TKey, TValidationError>
   where TKey : IParsable<TKey>
   where TValidationError : class, IValidationError<TValidationError>
{
   private readonly T? _item;
   public T? Item => Error is null ? _item : throw new ValidationException(Error);

   public string? Error { get; }

   private BoundValueObject(T? item)
   {
      _item = item;
   }

   private BoundValueObject(string error)
   {
      Error = error ?? throw new ArgumentNullException(nameof(error));
   }

   public static bool TryParse(string s, IFormatProvider? formatProvider, out BoundValueObject<T, TKey, TValidationError> value)
   {
      if (!TKey.TryParse(s, formatProvider, out var key))
      {
         value = new BoundValueObject<T, TKey, TValidationError>($"The value '{s}' cannot be converted to '{typeof(TKey).FullName}'.");
      }
      else
      {
         var validationError = T.Validate(key, formatProvider, out var item);

         if (validationError is null || item is IValidatableEnum)
         {
            value = new BoundValueObject<T, TKey, TValidationError>(item);
         }
         else
         {
            value = new BoundValueObject<T, TKey, TValidationError>(validationError.ToString() ?? "Invalid format");
         }
      }

      return true;
   }
}

public class BoundValueObject<T, TValidationError> : IBoundParam
   where T : IValueObjectFactory<T, string, TValidationError>
   where TValidationError : class, IValidationError<TValidationError>
{
   private readonly T? _item;
   public T? Value => Error is null ? _item : throw new ValidationException(Error);

   public string? Error { get; }

   private BoundValueObject(T? item)
   {
      _item = item;
   }

   private BoundValueObject(string error)
   {
      Error = error ?? throw new ArgumentNullException(nameof(error));
   }

   public static bool TryParse(string s, IFormatProvider? formatProvider, out BoundValueObject<T, TValidationError> value)
   {
      var validationError = T.Validate(s, formatProvider, out var item);

      if (validationError is null || item is IValidatableEnum)
      {
         value = new BoundValueObject<T, TValidationError>(item);
      }
      else
      {
         value = new BoundValueObject<T, TValidationError>(validationError.ToString() ?? "Invalid format");
      }

      return true;
   }
}

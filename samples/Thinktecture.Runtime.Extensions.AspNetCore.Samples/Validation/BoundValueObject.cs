using System;
using System.ComponentModel.DataAnnotations;

namespace Thinktecture.Validation;

public class BoundValueObject<T, TKey> : IBoundParam
   where T : IValueObjectFactory<T, TKey>
   where TKey : IParsable<TKey>
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

   public static bool TryParse(string s, IFormatProvider? formatProvider, out BoundValueObject<T, TKey> value)
   {
      if (!TKey.TryParse(s, formatProvider, out var key))
      {
         value = new BoundValueObject<T, TKey>($"The value '{s}' cannot be converted to '{typeof(TKey).FullName}'.");
      }
      else
      {
         var validationResult = T.Validate(key, formatProvider, out var item);

         if (validationResult is null || item is IValidatableEnum<TKey>)
         {
            value = new BoundValueObject<T, TKey>(item);
         }
         else
         {
            value = new BoundValueObject<T, TKey>(validationResult.ErrorMessage ?? $"The value '{s}' cannot be converted to '{typeof(T).FullName}'.");
         }
      }

      return true;
   }
}

public class BoundValueObject<T> : IBoundParam
   where T : IValueObjectFactory<T, string>
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

   public static bool TryParse(string s, IFormatProvider? formatProvider, out BoundValueObject<T> value)
   {
      var validationResult = T.Validate(s, formatProvider, out var item);

      if (validationResult is null || item is IValidatableEnum<string>)
      {
         value = new BoundValueObject<T>(item);
      }
      else
      {
         value = new BoundValueObject<T>(validationResult.ErrorMessage ?? $"The value '{s}' cannot be converted to '{typeof(T).FullName}'.");
      }

      return true;
   }
}

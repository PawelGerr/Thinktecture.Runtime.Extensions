using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Thinktecture
{
   /// <summary>
   /// Base class for enum-like classes.
   /// </summary>
   /// <typeparam name="TKey">Type of the key.</typeparam>
   public interface IValidatableEnum<out TKey> : IEnum<TKey>, IValidatableObject
      where TKey : notnull
   {
      /// <summary>
      /// Indication whether the current enumeration item is valid or not.
      /// </summary>
      bool IsValid => throw new NotImplementedException("This property will be implemented by the source generator.");

      /// <summary>
      /// Checks whether current enumeration item is valid.
      /// </summary>
      /// <exception cref="InvalidOperationException">The enumeration item is not valid.</exception>
      void EnsureValid()
      {
         throw new NotImplementedException("This method will be implemented by the source generator.");
      }

      IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
      {
         return IsValid
                   ? Enumerable.Empty<ValidationResult>()
                   : new[] { new ValidationResult($"The enumeration item of type '{GetType().Name}' with identifier '{GetKey()}' is not valid.") };
      }
   }
}

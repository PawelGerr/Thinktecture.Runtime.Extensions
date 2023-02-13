using System.ComponentModel.DataAnnotations;

namespace Thinktecture;

/// <summary>
/// Interface of Smart Enum.
/// </summary>
/// <typeparam name="TKey">Type of the key.</typeparam>
public interface IValidatableEnum<TKey> : IEnum<TKey>, IValidatableObject
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
   void EnsureValid() => throw new NotImplementedException("This method will be implemented by the source generator.");

   IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
   {
      return IsValid
                ? Enumerable.Empty<ValidationResult>()
                : new[] { new ValidationResult($"There is no item of type '{GetType().Name}' with the identifier '{GetKey()}'.") };
   }
}

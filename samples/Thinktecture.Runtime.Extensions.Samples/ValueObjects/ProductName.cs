using System;
using System.ComponentModel.DataAnnotations;

namespace Thinktecture.ValueObjects;

[ValueObject(NullInFactoryMethodsYieldsNull = true)]
public partial class ProductName
{
   [ValueObjectEqualityMember(EqualityComparer = nameof(StringComparer.OrdinalIgnoreCase))]
   private string Value { get; }

   static partial void ValidateFactoryArguments(ref ValidationResult? validationResult, ref string value)
   {
      if (String.IsNullOrWhiteSpace(value))
      {
         validationResult = new ValidationResult("Product name cannot be empty.");
         return;
      }

      if (value.Length == 1)
      {
         validationResult = new ValidationResult("Product name cannot be 1 character long.");
         return;
      }

      value = value.Trim();
   }
}

using System;
using System.ComponentModel.DataAnnotations;

namespace Thinktecture;

[ValueObject(NullInFactoryMethodsYieldsNull = true)]
public sealed partial class ProductNameWithMessagePackFormatter
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

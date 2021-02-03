using System;
using System.ComponentModel.DataAnnotations;

namespace Thinktecture.ValueTypes
{
   [ValueType]
   public partial class ProductName
   {
      [ValueTypeEqualityMember(EqualityComparer = nameof(StringComparer.OrdinalIgnoreCase))]
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
}

using System;
using System.ComponentModel.DataAnnotations;

namespace Thinktecture.Database;

[ValueObject]
public sealed partial class Description
{
   private readonly string _value;

   static partial void ValidateFactoryArguments(ref ValidationResult? validationResult, ref string value)
   {
      if (String.IsNullOrWhiteSpace(value))
      {
         value = null!;
         validationResult = new ValidationResult("Description cannot be empty.");
         return;
      }

      value = value.Trim();

      if (value.Length < 2)
         validationResult = new ValidationResult("Description cannot be less than 2 characters.");
   }
}

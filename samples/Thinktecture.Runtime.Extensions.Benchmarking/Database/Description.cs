using System;

namespace Thinktecture.Database;

[ValueObject]
public sealed partial class Description
{
   private readonly string _value;

   static partial void ValidateFactoryArguments(ref ValidationError? validationError, ref string value)
   {
      if (String.IsNullOrWhiteSpace(value))
      {
         value = null!;
         validationError = new ValidationError("Description cannot be empty.");
         return;
      }

      value = value.Trim();

      if (value.Length < 2)
         validationError = new ValidationError("Description cannot be less than 2 characters.");
   }
}

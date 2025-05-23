using System;

namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ComplexValueObject(DefaultStringComparison = StringComparison.OrdinalIgnoreCase)]
public partial class BoundaryWithStrings
{
   public string Lower { get; }
   public string? Upper { get; }

   static partial void ValidateFactoryArguments(ref ValidationError? validationError, ref string lower, ref string? upper)
   {
      if (String.IsNullOrWhiteSpace(lower))
      {
         validationError = new ValidationError("Lower boundary must not be empty.");
         return;
      }

      if (upper is null || lower.Length <= upper.Length)
         return;

      validationError = new ValidationError($"The length of lower boundary '{lower}' must be less than the length of the upper boundary '{upper}'");
   }
}

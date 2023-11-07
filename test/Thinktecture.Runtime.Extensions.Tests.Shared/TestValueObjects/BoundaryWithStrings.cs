using System.ComponentModel.DataAnnotations;

namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ValueObject]
public sealed partial class BoundaryWithStrings
{
   public string Lower { get; }
   public string? Upper { get; }

   static partial void ValidateFactoryArguments(ref ValidationResult? validationResult, ref string lower, ref string? upper)
   {
      if (lower is null)
      {
         lower = null!;
         validationResult = new ValidationResult("Lower boundary must not be null.");
         return;
      }

      if (upper is null || lower.Length <= upper.Length)
         return;

      validationResult = new ValidationResult($"The length of lower boundary '{lower}' must be less than the length of the upper boundary '{upper}'");
   }
}

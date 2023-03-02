using System.ComponentModel.DataAnnotations;

namespace Thinktecture.ValueObjects;

[ValueObject]
public sealed partial class Amount
{
   private readonly int _value;

   static partial void ValidateFactoryArguments(ref ValidationResult? validationResult, ref int value)
   {
      if (value < 0)
         validationResult = new ValidationResult("Amount must be positive.");
   }
}

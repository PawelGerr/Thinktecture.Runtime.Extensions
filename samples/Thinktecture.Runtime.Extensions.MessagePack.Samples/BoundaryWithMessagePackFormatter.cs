using System.ComponentModel.DataAnnotations;

namespace Thinktecture;

[ValueObject]
public partial class BoundaryWithMessagePackFormatter
{
   public decimal Lower { get; }
   public decimal Upper { get; }

   static partial void ValidateFactoryArguments(ref ValidationResult? validationResult, ref decimal lower, ref decimal upper)
   {
      if (lower <= upper)
         return;

      validationResult = new ValidationResult($"Lower boundary '{lower}' must be less than upper boundary '{upper}'");
   }
}

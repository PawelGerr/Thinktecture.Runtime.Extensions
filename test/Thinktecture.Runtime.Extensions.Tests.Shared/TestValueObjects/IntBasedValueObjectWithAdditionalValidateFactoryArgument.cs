namespace Thinktecture.Runtime.Tests.TestValueObjects;

// Probe for the "additional ValidateFactoryArguments parameter" feature (keyed value object).
// Demonstrates a nullable reference-type additional parameter (synthesized '= null') declared WITHOUT a default.
[ValueObject<int>]
public partial class IntBasedValueObjectWithAdditionalValidateFactoryArgument
{
   static partial void ValidateFactoryArguments(ref ValidationError? validationError, ref int value, int? clampMax)
   {
      if (value < 0)
      {
         validationError = new ValidationError("Value cannot be negative");
         return;
      }

      if (clampMax is { } max && value > max)
         value = max;
   }

   // Hand-written factory threading the extra argument; the generated 'Create(int)' omits it (synthesized '= null').
   public static IntBasedValueObjectWithAdditionalValidateFactoryArgument Create(int value, int? clampMax)
   {
      ValidationError? validationError = null;
      ValidateFactoryArguments(ref validationError, ref value, clampMax);

      if (validationError is not null)
         throw new System.ComponentModel.DataAnnotations.ValidationException(validationError.ToString() ?? "Validation failed.");

      return new IntBasedValueObjectWithAdditionalValidateFactoryArgument(value);
   }
}

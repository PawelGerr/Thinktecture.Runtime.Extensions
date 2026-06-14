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
   // Delegates to the generated throwing core 'CreateCore' - a compile-time gate proving the private core
   // exists and is accessible from the user's own partial declaration.
   public static IntBasedValueObjectWithAdditionalValidateFactoryArgument Create(int value, int? clampMax)
      => CreateCore(value, clampMax);

   // Thin public passthrough to the generated non-throwing core 'ValidateCore' (try-semantics: error is null).
   public static ValidationError? TryValidate(int value, int? clampMax, out IntBasedValueObjectWithAdditionalValidateFactoryArgument? obj)
      => ValidateCore(value, clampMax, out obj);
}

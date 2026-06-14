namespace Thinktecture.Runtime.Tests.TestValueObjects;

// Probe for the "additional ValidateFactoryArguments parameter" feature (complex value object).
// The additional parameter 'allowEqual' is declared WITHOUT a default value - the source generator
// reproduces it on its own declaration and synthesizes the default there.
[ComplexValueObject]
public partial class BoundaryWithAdditionalValidateFactoryArgument
{
   public decimal Lower { get; }
   public decimal Upper { get; }

   static partial void ValidateFactoryArguments(
      ref ValidationError? validationError,
      ref decimal lower,
      ref decimal upper,
      bool allowEqual)
   {
      var isValid = allowEqual ? lower <= upper : lower < upper;

      if (!isValid)
         validationError = new ValidationError($"Lower boundary '{lower}' must be less than{(allowEqual ? " or equal to" : "")} upper boundary '{upper}'");
   }

   // Hand-written factory threading the extra argument through the hook (the generated 'Create(decimal, decimal)'
   // omits it, so the synthesized default - 'default(bool)' = false - applies there).
   // Delegates to the generated throwing core 'CreateCore' - a compile-time gate proving the private core
   // exists and is accessible from the user's own partial declaration.
   public static BoundaryWithAdditionalValidateFactoryArgument Create(decimal lower, decimal upper, bool allowEqual)
      => CreateCore(lower, upper, allowEqual);
}

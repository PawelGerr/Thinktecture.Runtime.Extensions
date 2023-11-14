namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ValueObject]
[ValueObjectValidationError<BoundaryValidationError>]
public sealed partial class BoundaryWithCustomError
{
   public decimal Lower { get; }
   public decimal Upper { get; }

   static partial void ValidateFactoryArguments(ref BoundaryValidationError? validationError, ref decimal lower, ref decimal upper)
   {
      if (lower <= upper)
         return;

      validationError = new BoundaryValidationError($"Lower boundary '{lower}' must be less than upper boundary '{upper}'");
   }
}

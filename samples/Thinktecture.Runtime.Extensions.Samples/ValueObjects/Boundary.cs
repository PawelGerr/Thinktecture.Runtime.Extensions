namespace Thinktecture.ValueObjects;

[ComplexValueObject]
[ValueObjectValidationError<BoundaryValidationError>]
public partial class Boundary
{
   public decimal Lower { get; }
   public decimal Upper { get; }

   static partial void ValidateFactoryArguments(ref BoundaryValidationError? validationError, ref decimal lower, ref decimal upper)
   {
      if (lower <= upper)
         return;

      validationError = new BoundaryValidationError("Lower boundary must be less than upper boundary.", lower, upper);
   }
}

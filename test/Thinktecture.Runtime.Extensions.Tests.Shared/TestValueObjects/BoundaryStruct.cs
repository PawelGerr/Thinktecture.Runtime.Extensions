namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ComplexValueObject]
public partial struct BoundaryStruct
{
   public decimal Lower { get; }
   public decimal Upper { get; }

   static partial void ValidateFactoryArguments(ref ValidationError? validationError, ref decimal lower, ref decimal upper)
   {
      if (lower <= upper)
         return;

      validationError = new ValidationError($"Lower boundary '{lower}' must be less than upper boundary '{upper}'");
   }
}
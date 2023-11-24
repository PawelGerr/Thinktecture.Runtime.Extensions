namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ValueObject(CreateFactoryMethodName = "Get",
             TryCreateFactoryMethodName = "TryGet")]
public sealed partial class BoundaryWithCustomFactoryNames
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

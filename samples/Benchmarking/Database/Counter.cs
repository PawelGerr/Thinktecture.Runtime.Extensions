namespace Thinktecture.Database;

[ValueObject<int>]
public partial class Counter
{
   static partial void ValidateFactoryArguments(ref ValidationError? validationError, ref int value)
   {
      if (value < 0)
         validationError = new ValidationError("Counter cannot be negative.");
   }
}

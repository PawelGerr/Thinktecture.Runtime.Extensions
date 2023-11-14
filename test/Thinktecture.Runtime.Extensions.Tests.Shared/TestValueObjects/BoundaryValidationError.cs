namespace Thinktecture.Runtime.Tests.TestValueObjects;

public class BoundaryValidationError : IValidationError<BoundaryValidationError>
{
   public string Message { get; }

   public BoundaryValidationError(string message)
   {
      Message = message;
   }

   public static BoundaryValidationError Create(string message)
   {
      return new BoundaryValidationError(message);
   }

   public override string ToString()
   {
      return Message;
   }
}

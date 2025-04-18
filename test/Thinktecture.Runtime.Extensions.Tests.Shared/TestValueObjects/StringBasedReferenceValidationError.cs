namespace Thinktecture.Runtime.Tests.TestValueObjects;

public class StringBasedReferenceValidationError : IValidationError<StringBasedReferenceValidationError>
{
   public string Message { get; }

   public StringBasedReferenceValidationError(string message)
   {
      Message = message;
   }

   public static StringBasedReferenceValidationError Create(string message)
   {
      return new StringBasedReferenceValidationError(message);
   }

   public override string ToString()
   {
      return Message;
   }
}

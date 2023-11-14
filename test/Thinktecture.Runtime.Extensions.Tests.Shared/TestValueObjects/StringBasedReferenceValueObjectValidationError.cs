namespace Thinktecture.Runtime.Tests.TestValueObjects;

public class StringBasedReferenceValueObjectValidationError : IValidationError<StringBasedReferenceValueObjectValidationError>
{
   public string Message { get; }

   public StringBasedReferenceValueObjectValidationError(string message)
   {
      Message = message;
   }

   public static StringBasedReferenceValueObjectValidationError Create(string message)
   {
      return new StringBasedReferenceValueObjectValidationError(message);
   }

   public override string ToString()
   {
      return Message;
   }
}

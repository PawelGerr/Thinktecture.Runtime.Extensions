namespace Thinktecture.Runtime.Tests.TestEnums;

public class CustomValidationError : IValidationError<CustomValidationError>
{
   public string Message { get; }

   public CustomValidationError(string message)
   {
      Message = message;
   }

   public static CustomValidationError Create(string message)
   {
      return new CustomValidationError(message);
   }

   public override string ToString()
   {
      return Message;
   }
}

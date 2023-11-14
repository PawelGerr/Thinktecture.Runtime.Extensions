namespace Thinktecture.Runtime.Tests.TestEnums;

public class TestEnumValidationError : IValidationError<TestEnumValidationError>
{
   public string Message { get; }

   public TestEnumValidationError(string message)
   {
      Message = message;
   }

   public static TestEnumValidationError Create(string message)
   {
      return new TestEnumValidationError(message);
   }

   public override string ToString()
   {
      return Message;
   }
}

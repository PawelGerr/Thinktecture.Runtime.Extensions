namespace Thinktecture.SmartEnums;

public class ProductTypeValidationError : IValidationError<ProductTypeValidationError>
{
   public string Message { get; }

   private ProductTypeValidationError(string message)
   {
      Message = message;
   }

   public static ProductTypeValidationError Create(string message)
   {
      return new ProductTypeValidationError(message);
   }

   public override string ToString()
   {
      return Message;
   }
}

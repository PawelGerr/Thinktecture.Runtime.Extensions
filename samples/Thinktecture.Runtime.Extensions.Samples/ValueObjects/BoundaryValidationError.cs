namespace Thinktecture.ValueObjects;

public class BoundaryValidationError : IValidationError<BoundaryValidationError>
{
   public string Message { get; }
   public decimal? Lower { get; }
   public decimal? Upper { get; }

   public BoundaryValidationError(
      string message,
      decimal? lower,
      decimal? upper)
   {
      Message = message;
      Lower = lower;
      Upper = upper;
   }

   public static BoundaryValidationError Create(string message)
   {
      return new BoundaryValidationError(message, null, null);
   }

   public override string ToString()
   {
      return $"{Message} (Lower={Lower}|Upper={Upper})";
   }
}

using System;

namespace Thinktecture.ValueObjects;

/// <summary>
/// Represents a time period with a definite start date and an open-ended end date.
/// </summary>
[ComplexValueObject]
public partial struct Period
{
   /// <summary>
   /// The definite start date of the period.
   /// </summary>
   public DateOnly From { get; }

   /// <summary>
   /// The open-ended end date of the period.
   /// </summary>
   public OpenEndDate Until { get; }

   static partial void ValidateFactoryArguments(
      ref ValidationError? validationError,
      ref DateOnly from,
      ref OpenEndDate until
   )
   {
      if (from >= until)
         validationError = new ValidationError("From must be earlier than Until");
   }

   public bool IntersectsWith(Period other)
   {
      return From <= other.Until && other.From <= Until;
   }
}

using System;
using System.ComponentModel.DataAnnotations;

namespace Thinktecture.ValueObjects;

/// <summary>
/// Represents a day and month without a year, useful for recurring dates like birthdays or anniversaries.
/// </summary>
/// <remarks>
/// Use <see cref="DateOnly"/> as the underlying type
/// * to ensure that the day and month are valid
/// * for comparison purposes
/// * for easier EF Core support
/// </remarks>
[ValueObject<DateOnly>(
   ConversionFromKeyMemberType = ConversionOperatorsGeneration.Implicit, // Cast from DateOnly to DayMonth
   ConversionToKeyMemberType = ConversionOperatorsGeneration.None,       // No cast from DayMonth to DateOnly
   SkipToString = true)]
public readonly partial struct DayMonth
{
   // Use year 2000 because it is a leap year and for correct comparisons
   private const int _REFERENCE_YEAR = 2000;

   public int Day => _value.Day;
   public int Month => _value.Month;

   public static DayMonth Create(int month, int day)
   {
      try
      {
         var date = new DateOnly(_REFERENCE_YEAR, month, day);
         return new DayMonth(date);
      }
      catch (Exception ex)
      {
         throw new ValidationException($"Invalid day '{day}' or month '{month}'.", ex);
      }
   }

   static partial void ValidateFactoryArguments(ref ValidationError? validationError, ref DateOnly value)
   {
      // Normalize: strip the year, keeping only month and day
      if (value.Year != _REFERENCE_YEAR)
         value = new DateOnly(_REFERENCE_YEAR, value.Month, value.Day);
   }

   public override string ToString()
   {
      return _value.ToString("M");
   }
}

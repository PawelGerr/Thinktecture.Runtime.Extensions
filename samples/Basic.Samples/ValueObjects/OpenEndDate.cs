using System;
using System.Globalization;

namespace Thinktecture.ValueObjects;

[ValueObject<DateOnly>(SkipKeyMember = true,                                                          // We implement the key member "Date" ourselves
                       KeyMemberName = nameof(Date),                                                  // Source Generator needs to know the name we've chosen
                       DefaultInstancePropertyName = "Infinite",                                      // "EndDate.Infinite" represent an open-ended end date
                       EqualityComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads, // for comparison with DateOnly without implicit cast
                       ComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                       AllowDefaultStructs = true,
                       SkipToString = true)]
public partial struct OpenEndDate
{
   private readonly DateOnly? _date;

   // can be public as well
   private DateOnly Date
   {
      get => _date ?? DateOnly.MaxValue;
      init => _date = value;
   }

   /// <summary>
   /// Gets a value indicating whether the current date represents December 31st of any year.
   /// </summary>
   public bool IsEndOfYear => this != Infinite && Date is (_, 12, 31);

   /// <summary>
   /// Creates an open-ended date with the specified year, month and day.
   /// </summary>
   public static OpenEndDate Create(int year, int month, int day)
   {
      return Create(new DateOnly(year, month, day));
   }

   /// <summary>
   /// Creates an open-ended date from <see cref="DateTime"/>.
   /// </summary>
   public static OpenEndDate Create(DateTime dateTime)
   {
      return Create(dateTime.Year, dateTime.Month, dateTime.Day);
   }

   static partial void ValidateFactoryArguments(
      ref ValidationError? validationError,
      ref DateOnly date
   )
   {
      if (date == DateOnly.MinValue)
         validationError = new ValidationError("The end date cannot be DateOnly.MinValue.");
   }

   /// <summary>
   /// Adjusts the current date to the last day of the month.
   /// </summary>
   public OpenEndDate MoveToEndOfMonth()
   {
      if (this == Infinite)
         return this;

      var days = DateTime.DaysInMonth(Date.Year, Date.Month);

      return days == Date.Day
                ? this
                : Create(Date.Year, Date.Month, days);
   }

   public override string ToString() =>
      this == Infinite ? "Infinite" : Date.ToString("O", CultureInfo.InvariantCulture);
}

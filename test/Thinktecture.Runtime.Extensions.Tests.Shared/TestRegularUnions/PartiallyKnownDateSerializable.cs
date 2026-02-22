using System;

namespace Thinktecture.Runtime.Tests.TestRegularUnions;

[Union]
[ObjectFactory<string>(
   UseForSerialization = SerializationFrameworks.All,
   UseForModelBinding = true,
   UseWithEntityFramework = true)]
public abstract partial record PartiallyKnownDateSerializable
{
   public int Year { get; }

   private PartiallyKnownDateSerializable(int year)
   {
      Year = year;
   }

   public sealed record YearOnly(int Year) : PartiallyKnownDateSerializable(Year);

   public sealed record YearMonth(int Year, int Month) : PartiallyKnownDateSerializable(Year);

   public sealed record Date(int Year, int Month, int Day) : PartiallyKnownDateSerializable(Year);

   public static implicit operator PartiallyKnownDateSerializable(DateOnly dateOnly) =>
      new Date(dateOnly.Year, dateOnly.Month, dateOnly.Day);

   public static implicit operator PartiallyKnownDateSerializable?(DateOnly? dateOnly) =>
      dateOnly is null ? null : dateOnly.Value;

   public static ValidationError? Validate(string? value, IFormatProvider? provider, out PartiallyKnownDateSerializable? item)
   {
      if (value is null)
      {
         item = null;
         return null;
      }

      var parts = value.Split('-');

      item = parts.Length switch
      {
         1 => new YearOnly(int.Parse(parts[0], provider)),
         2 => new YearMonth(int.Parse(parts[0], provider), int.Parse(parts[1], provider)),
         3 => new Date(int.Parse(parts[0], provider), int.Parse(parts[1], provider), int.Parse(parts[2], provider)),
         _ => null
      };

      return item is null ? new ValidationError("Invalid date format. Expected 'YYYY-MM' or 'YYYY-MM-DD'.") : null;
   }

   public string ToValue()
   {
      return Switch(
         yearOnly: yearOnly => yearOnly.Year.ToString("D4"),
         yearMonth: yearMonth => $"{yearMonth.Year:D4}-{yearMonth.Month:D2}",
         date: date => $"{date.Year:D4}-{date.Month:D2}-{date.Day:D2}"
      );
   }
}

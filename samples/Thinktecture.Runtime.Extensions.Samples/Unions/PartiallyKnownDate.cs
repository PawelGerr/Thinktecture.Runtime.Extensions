using System;
using System.Text.Json.Serialization;

namespace Thinktecture.Unions;

[JsonDerivedType(typeof(YearOnly), "Year")]
[JsonDerivedType(typeof(YearMonth), "YearMonth")]
[JsonDerivedType(typeof(Date), "Date")]
[Union]
public abstract partial record PartiallyKnownDate
{
   public int Year { get; }

   private PartiallyKnownDate(int year)
   {
      Year = year;
   }

   public sealed record YearOnly(int Year) : PartiallyKnownDate(Year);

   public sealed record YearMonth(int Year, int Month) : PartiallyKnownDate(Year);

   public sealed record Date(int Year, int Month, int Day) : PartiallyKnownDate(Year);

   public static implicit operator PartiallyKnownDate(DateOnly dateOnly) =>
      new Date(dateOnly.Year, dateOnly.Month, dateOnly.Day);

   public static implicit operator PartiallyKnownDate?(DateOnly? dateOnly) =>
      dateOnly is null ? null : dateOnly.Value;
}

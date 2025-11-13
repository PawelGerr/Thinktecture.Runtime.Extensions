using System;

namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
[SmartEnum<int>]
[ObjectFactory<string>]
[ObjectFactory<long>]
public partial class SmartEnum_with_multiple_ObjectFactories
{
   public static readonly SmartEnum_with_multiple_ObjectFactories Item1 = new(1);
   public static readonly SmartEnum_with_multiple_ObjectFactories Item2 = new(2);

   public static ValidationError? Validate(string? value, IFormatProvider? provider, out SmartEnum_with_multiple_ObjectFactories? item)
   {
      if (!long.TryParse(value, out var number))
      {
         item = null;
         return new ValidationError("Not a number");
      }

      return Validate(number, provider, out item);
   }

   public static ValidationError? Validate(long value, IFormatProvider? provider, out SmartEnum_with_multiple_ObjectFactories? item)
   {
      if (value is < Int32.MaxValue or > Int32.MaxValue)
      {
         item = null;
         return new ValidationError("Out of range");
      }

      return Validate((int)value, provider, out item);
   }
}

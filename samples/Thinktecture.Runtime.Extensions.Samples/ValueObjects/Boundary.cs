using System;
using System.ComponentModel.DataAnnotations;

namespace Thinktecture.ValueObjects;

[ValueObject]
[ValueObjectFactory<string>]
[ValueObjectFactory<ValueTuple<int, int>>]
public sealed partial class Boundary
{
   public decimal Lower { get; }
   public decimal Upper { get; }

   static partial void ValidateFactoryArguments(ref ValidationResult? validationResult, ref decimal lower, ref decimal upper)
   {
      if (lower <= upper)
         return;

      validationResult = new ValidationResult($"Lower boundary '{lower}' must be less than upper boundary '{upper}'",
                                              new[] { nameof(Lower), nameof(Upper) });
   }

   /// <summary>
   /// Custom implementation of "IValueObjectFactory&lt;Boundary, string&gt;"
   /// </summary>
   public static ValidationResult? Validate(string? value, IFormatProvider? provider, out Boundary? item)
   {
      item = null;

      if (value is null)
         return ValidationResult.Success;

      var parts = value.Split(":", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

      if (parts.Length != 2)
         return new ValidationResult("Invalid format.");

      if (!Decimal.TryParse(parts[0], provider, out var lower) || !Decimal.TryParse(parts[1], provider, out var upper))
         return new ValidationResult("The provided values are not numbers.");

      return Validate(lower, upper, out item);
   }

   /// <summary>
   /// Custom implementation of "IValueObjectFactory&lt;Boundary, (int, int)&gt;"
   /// </summary>
   public static ValidationResult? Validate((int, int) value, IFormatProvider? provider, out Boundary? item)
   {
      return Validate(value.Item1, value.Item2, out item);
   }
}

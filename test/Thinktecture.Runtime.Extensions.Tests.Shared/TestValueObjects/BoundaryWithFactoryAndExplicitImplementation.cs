using System;
using System.ComponentModel.DataAnnotations;

namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ValueObject]
[ValueObjectFactory<string>(UseForSerialization = SerializationFrameworks.All)]
public sealed partial class BoundaryWithFactoryAndExplicitImplementation
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

   static ValidationResult? IValueObjectFactory<BoundaryWithFactoryAndExplicitImplementation, string>.Validate(string? value, IFormatProvider? provider, out BoundaryWithFactoryAndExplicitImplementation? item)
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

   string IValueObjectConverter<string>.ToValue()
   {
      return $"{Lower}:{Upper}";
   }
}

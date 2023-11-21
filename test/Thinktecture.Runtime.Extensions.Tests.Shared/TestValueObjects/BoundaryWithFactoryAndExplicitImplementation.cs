using System;

namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ValueObject]
[ValueObjectFactory<string>(UseForSerialization = SerializationFrameworks.All)]
public sealed partial class BoundaryWithFactoryAndExplicitImplementation
{
   public decimal Lower { get; }
   public decimal Upper { get; }

   static partial void ValidateFactoryArguments(ref ValidationError? validationError, ref decimal lower, ref decimal upper)
   {
      if (lower <= upper)
         return;

      validationError = new ValidationError($"Lower boundary '{lower}' must be less than upper boundary '{upper}'");
   }

   static ValidationError? IValueObjectFactory<BoundaryWithFactoryAndExplicitImplementation, string, ValidationError>.Validate(string? value, IFormatProvider? provider, out BoundaryWithFactoryAndExplicitImplementation? item)
   {
      item = null;

      if (value is null)
         return null;

      var parts = value.Split(":", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

      if (parts.Length != 2)
         return new ValidationError("Invalid format.");

      if (!Decimal.TryParse(parts[0], provider, out var lower) || !Decimal.TryParse(parts[1], provider, out var upper))
         return new ValidationError("The provided values are not numbers.");

      return Validate(lower, upper, out item);
   }

   string IValueObjectConvertable<string>.ToValue()
   {
      return $"{Lower}:{Upper}";
   }
}

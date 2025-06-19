using System;

namespace Thinktecture.ValueObjects;

/// <summary>
/// Represents a boundary with lower and upper decimal values, supporting custom object factories for string and tuple inputs.
/// </summary>
[ComplexValueObject]
[ObjectFactory<string>(UseForSerialization = SerializationFrameworks.All)]
[ObjectFactory<ValueTuple<int, int>>]
public partial class BoundaryWithFactories
{
   public decimal Lower { get; }
   public decimal Upper { get; }

   static partial void ValidateFactoryArguments(ref ValidationError? validationError, ref decimal lower, ref decimal upper)
   {
      if (lower <= upper)
         return;

      validationError = new ValidationError($"Lower boundary '{lower}' must be less than upper boundary '{upper}'");
   }

   /// <summary>
   /// Custom implementation of "IObjectFactory&lt;Boundary, string&gt;"
   /// requested by "ObjectFactory&lt;string&gt;".
   /// </summary>
   public static ValidationError? Validate(string? value, IFormatProvider? provider, out BoundaryWithFactories? item)
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

   /// <summary>
   /// Custom implementation of "IConvertible&amp;lt;string&amp;gt;"
   /// </summary>
   public string ToValue()
   {
      return $"{Lower}:{Upper}";
   }

   /// <summary>
   /// Custom implementation of "IObjectFactory&lt;Boundary, (int, int)&gt;"
   /// </summary>
   public static ValidationError? Validate((int, int) value, IFormatProvider? provider, out BoundaryWithFactories? item)
   {
      return Validate(value.Item1, value.Item2, out item);
   }
}

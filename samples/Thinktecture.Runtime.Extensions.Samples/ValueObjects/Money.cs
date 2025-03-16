using System.Numerics;
using Thinktecture.SmartEnums;

namespace Thinktecture.ValueObjects;

/// <summary>
/// Represents a monetary amount that is always positive and rounded to 2 decimal places.
/// </summary>
/// <remarks>
/// Multiplication and division need special handling because they can lead to more than 2 decimal places.
/// In that case the developer has to decide the rounding strategy.
/// </remarks>
[ValueObject<decimal>(
   AllowDefaultStructs = true,
   DefaultInstancePropertyName = "Zero",
   MultiplyOperators = OperatorsGeneration.None,
   DivisionOperators = OperatorsGeneration.None)]
public readonly partial struct Money
   : IMultiplyOperators<Money, int, Money> // Multiplication with int don't lead to more than 2 decimal places
{
   static partial void ValidateFactoryArguments(ref ValidationError? validationError, ref decimal value)
   {
      if (value < 0)
      {
         validationError = new ValidationError("Amount cannot be negative");
         return;
      }

      value = MoneyRoundingStrategy.Default.Round(value);
   }

   public static Money? Create(decimal? amount, MoneyRoundingStrategy roundingStrategy)
   {
      return amount is null ? null : Create(amount.Value, roundingStrategy);
   }

   public static Money Create(decimal amount, MoneyRoundingStrategy roundingStrategy)
   {
      return Create(roundingStrategy.Round(amount));
   }

   public static Money operator *(Money left, int right)
   {
      return Create(left._value * right);
   }

   public static Money operator *(int right, Money left)
   {
      return Create(left._value * right);
   }
}

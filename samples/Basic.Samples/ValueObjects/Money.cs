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
   // The additional 'roundingStrategy' parameter is declared WITHOUT a default value.
   // The source generator reproduces it on its own declaration and supplies the default (= null) there,
   // so the generated 'Create(decimal)' (which omits the argument) falls back to the default strategy.
   static partial void ValidateFactoryArguments(ref ValidationError? validationError, ref decimal value, MoneyRoundingStrategy? roundingStrategy)
   {
      if (value < 0)
      {
         validationError = new ValidationError("Amount cannot be negative");
         return;
      }

      // Rounding happens exactly once - here in the hook - regardless of which factory method is used.
      value = (roundingStrategy ?? MoneyRoundingStrategy.Default).Round(value);
   }

   public static Money? Create(decimal? amount, MoneyRoundingStrategy roundingStrategy)
      => amount is null ? null : CreateCore(amount.Value, roundingStrategy);

   public static Money Create(decimal amount, MoneyRoundingStrategy roundingStrategy)
      => CreateCore(amount, roundingStrategy);

   public static Money operator *(Money left, int right)
   {
      return Create(left._value * right);
   }

   public static Money operator *(int right, Money left)
   {
      return Create(left._value * right);
   }
}

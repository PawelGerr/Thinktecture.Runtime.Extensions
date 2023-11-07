using System.ComponentModel.DataAnnotations;

namespace Thinktecture.ValueObjects;

[ValueObject(ComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
             AdditionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
             SubtractionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
             MultiplyOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
             DivisionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads)]
public sealed partial class AmountClass
{
   private readonly int _value;

   static partial void ValidateFactoryArguments(ref ValidationResult? validationResult, ref int value)
   {
      if (value < 0)
         validationResult = new ValidationResult("Amount must be positive.");
   }
}

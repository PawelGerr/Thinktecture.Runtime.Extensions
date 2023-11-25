namespace Thinktecture.ValueObjects;

[ValueObject<int>(ComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                  AdditionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                  SubtractionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                  MultiplyOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                  DivisionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads)]
public sealed partial class AmountClass
{
   static partial void ValidateFactoryArguments(ref ValidationError? validationError, ref int value)
   {
      if (value < 0)
         validationError = new ValidationError("Amount must be positive.");
   }
}

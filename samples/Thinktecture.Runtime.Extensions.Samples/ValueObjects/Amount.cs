namespace Thinktecture.ValueObjects;

[ValueObject<decimal>(DefaultInstancePropertyName = "Zero",                                  // renames Amount.Empty to Amount.Zero
                      ComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads, // for comparison of amount with a decimal without implicit conversion: amount > 42m
                      AdditionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,   // for arithmetic operations of amount with a decimal without implicit conversion: amount + 42m
                      SubtractionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                      MultiplyOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                      DivisionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads)]
public partial struct Amount
{
   static partial void ValidateFactoryArguments(ref ValidationError? validationError, ref decimal value)
   {
      if (value < 0)
         validationError = new ValidationError("Amount must be positive.");
   }
}

using System;

namespace Thinktecture.ValueObjects;

[ValueObject(DefaultInstancePropertyName = "Infinite",
             EqualityComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads)] // for comparison with DateOnly without implicit cast
public readonly partial struct EndDate
{
   // Source Generator should work with the property "Date" only and ignore this backing field
   [ValueObjectMemberIgnore]
   private readonly DateOnly? _date;

   // can be public as well
   private DateOnly Date
   {
      get => _date ?? DateOnly.MaxValue;
      init => _date = value;
   }

   // Further validation
   // static partial void ValidateFactoryArguments(ref ValidationResult? validationResult, ref DateOnly date)
   // {
   //    if (date.Year >= 2050)
   //       validationResult = new ValidationResult("The end date lies too far in the future.");
   // }
}

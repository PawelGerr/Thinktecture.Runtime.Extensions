using System;
using System.ComponentModel.DataAnnotations;

namespace Thinktecture.ValueObjects;

[ValueObject(DefaultInstancePropertyName = "Infinite",                                       // "EndDate.Infinite" represent an open-ended end date
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
   //    validationResult = date.Year switch
   //    {
   //       < 2000 => new ValidationResult("The end date lies too far in the past."),
   //       >= 2050 => new ValidationResult("The end date lies too far in the future."),
   //       _ => validationResult
   //    };
   // }
}

using System;

namespace Thinktecture.ValueObjects;

[ValueObject<DateOnly>(KeyMemberName = "Date",                                                         // Source Generator needs to know the name we've chosen
                       SkipKeyMember = true,                                                           // We implement the key member "Date" ourselves
                       DefaultInstancePropertyName = "Infinite",                                       // "EndDate.Infinite" represent an open-ended end date
                       EqualityComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads)] // for comparison with DateOnly without implicit cast
public readonly partial struct EndDate
{
   // Source Generator should work with the property "Date" only and ignore this backing field
   private readonly DateOnly? _date;

   // can be public as well
   private DateOnly Date
   {
      get => _date ?? DateOnly.MaxValue;
      init => _date = value;
   }

   // Further validation
   // static partial void ValidateFactoryArguments(ref ValidationError? validationError, ref DateOnly date)
   // {
   //    validationError = date.Year switch
   //    {
   //       < 2000 => new ValidationError("The end date lies too far in the past."),
   //       >= 2050 => new ValidationError("The end date lies too far in the future."),
   //       _ => validationError
   //    };
   // }
}

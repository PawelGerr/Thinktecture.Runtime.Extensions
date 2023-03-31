using System;

namespace Thinktecture.ValueObjects;

[ValueObject(DefaultInstancePropertyName = "Infinite")]
public readonly partial struct EndDate
{
   [ValueObjectMemberIgnore]
   private readonly DateOnly? _date;

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

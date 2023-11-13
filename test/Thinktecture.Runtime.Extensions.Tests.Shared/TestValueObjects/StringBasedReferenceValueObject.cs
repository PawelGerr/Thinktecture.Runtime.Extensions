using System;

namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ValueObject]
public sealed partial class StringBasedReferenceValueObject
{
   public string Property { get; }

   static partial void ValidateFactoryArguments(ref ValidationError? validationError, ref string property)
   {
      if (String.IsNullOrWhiteSpace(property))
      {
         property = null!;
         validationError = new ValidationError("Property cannot be empty.");
         return;
      }

      if (property.Length == 1)
         validationError = new ValidationError("Property cannot be 1 character long.");
   }
}

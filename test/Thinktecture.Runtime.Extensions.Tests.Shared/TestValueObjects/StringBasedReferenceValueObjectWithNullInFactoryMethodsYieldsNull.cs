using System;
using System.ComponentModel.DataAnnotations;

namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ValueObject(NullInFactoryMethodsYieldsNull = true)]
public sealed partial class StringBasedReferenceValueObjectWithNullInFactoryMethodsYieldsNull
{
   public string Property { get; }

   static partial void ValidateFactoryArguments(ref ValidationResult? validationResult, ref string property)
   {
      if (String.IsNullOrWhiteSpace(property))
      {
         property = null!;
         validationResult = new ValidationResult("Property cannot be empty.");
         return;
      }

      if (property.Length == 1)
         validationResult = new ValidationResult("Property cannot be 1 character long.");
   }
}

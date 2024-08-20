using System;

namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ValueObject<string>(KeyMemberKind = ValueObjectMemberKind.Property,
                     KeyMemberName = "Property",
                     KeyMemberAccessModifier = ValueObjectAccessModifier.Public)]
[ValueObjectValidationError<StringBasedReferenceValueObjectValidationError>]
public partial class StringBasedReferenceValueObjectWithCustomError
{
   static partial void ValidateFactoryArguments(ref StringBasedReferenceValueObjectValidationError? validationError, ref string property)
   {
      if (String.IsNullOrWhiteSpace(property))
      {
         property = null!;
         validationError = new StringBasedReferenceValueObjectValidationError("Property cannot be empty.");
         return;
      }

      if (property.Length == 1)
         validationError = new StringBasedReferenceValueObjectValidationError("Property cannot be 1 character long.");
   }
}

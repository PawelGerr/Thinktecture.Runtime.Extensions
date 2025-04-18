using System;

namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ValueObject<string>(KeyMemberKind = MemberKind.Property,
                     KeyMemberName = "Property",
                     KeyMemberAccessModifier = AccessModifier.Public)]
[ValidationError<StringBasedReferenceValidationError>]
[KeyMemberEqualityComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>]
[KeyMemberComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>]
public partial class StringBasedReferenceValueObjectWithCustomError
{
   static partial void ValidateFactoryArguments(ref StringBasedReferenceValidationError? validationError, ref string property)
   {
      if (String.IsNullOrWhiteSpace(property))
      {
         validationError = new StringBasedReferenceValidationError("Property cannot be empty.");
         return;
      }

      if (property.Length == 1)
         validationError = new StringBasedReferenceValidationError("Property cannot be 1 character long.");
   }
}

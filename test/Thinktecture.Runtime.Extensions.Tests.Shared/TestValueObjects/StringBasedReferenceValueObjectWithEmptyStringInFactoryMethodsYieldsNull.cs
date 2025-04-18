using System;

namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ValueObject<string>(KeyMemberKind = MemberKind.Property,
                     KeyMemberName = "Property",
                     KeyMemberAccessModifier = AccessModifier.Public,
                     EmptyStringInFactoryMethodsYieldsNull = true)]
[KeyMemberEqualityComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>]
[KeyMemberComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>]
public partial class StringBasedReferenceValueObjectWithEmptyStringInFactoryMethodsYieldsNull
{
   static partial void ValidateFactoryArguments(ref ValidationError? validationError, ref string property)
   {
      if (String.IsNullOrWhiteSpace(property))
      {
         validationError = new ValidationError("Property cannot be empty.");
         return;
      }

      if (property.Length == 1)
         validationError = new ValidationError("Property cannot be 1 character long.");
   }
}

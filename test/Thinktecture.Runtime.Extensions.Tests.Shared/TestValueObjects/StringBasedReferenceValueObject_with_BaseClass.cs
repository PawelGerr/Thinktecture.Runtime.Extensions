using System;

namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ValueObject<string>]
[KeyMemberEqualityComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>]
[KeyMemberComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>]
// ReSharper disable once InconsistentNaming
public partial class StringBasedReferenceValueObject_with_BaseClass : TestBaseClass
{
   static partial void ValidateFactoryArguments(ref ValidationError? validationError, ref string value)
   {
      if (String.IsNullOrWhiteSpace(value))
      {
         validationError = new ValidationError("Property cannot be empty.");
         return;
      }

      if (value.Length == 1)
         validationError = new ValidationError("Property cannot be 1 character long.");
   }
}

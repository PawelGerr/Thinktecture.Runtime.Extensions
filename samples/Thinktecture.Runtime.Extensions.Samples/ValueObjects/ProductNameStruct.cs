using System;

namespace Thinktecture.ValueObjects;

[ValueObject<string>]
[KeyMemberEqualityComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>]
[KeyMemberComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>]
public partial struct ProductNameStruct
{
   static partial void ValidateFactoryArguments(ref ValidationError? validationError, ref string value)
   {
      if (String.IsNullOrWhiteSpace(value))
      {
         validationError = new ValidationError("Product name cannot be empty.");
         return;
      }

      if (value.Length == 1)
      {
         validationError = new ValidationError("Product name cannot be 1 character long.");
         return;
      }

      value = value.Trim();
   }
}

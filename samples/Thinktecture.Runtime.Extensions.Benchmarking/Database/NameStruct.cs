using System;

namespace Thinktecture.Database;

[ValueObject<string>]
[ValueObjectKeyMemberEqualityComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>]
[ValueObjectKeyMemberComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>]
public partial struct NameStruct
{
   static partial void ValidateFactoryArguments(ref ValidationError? validationError, ref string value)
   {
      if (String.IsNullOrWhiteSpace(value))
      {
         validationError = new ValidationError("Name cannot be empty.");
         return;
      }

      value = value.Trim();

      if (value.Length < 2)
         validationError = new ValidationError("Name cannot be less than 2 characters.");
   }
}

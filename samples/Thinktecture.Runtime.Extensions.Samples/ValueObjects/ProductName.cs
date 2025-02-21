using System;

namespace Thinktecture.ValueObjects;

[ValueObject<string>(NullInFactoryMethodsYieldsNull = true)]
[ValueObjectKeyMemberEqualityComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>]
public partial class ProductName
{
   static partial void ValidateFactoryArguments(ref ValidationError? validationError, ref string value)
   {
      if (String.IsNullOrWhiteSpace(value))
      {
         value = null!;
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

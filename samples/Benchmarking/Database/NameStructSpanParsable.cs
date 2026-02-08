using System;

namespace Thinktecture.Database;

[ValueObject<string>]
[ObjectFactory<ReadOnlySpan<char>>(UseForSerialization = SerializationFrameworks.SystemTextJson)]
[KeyMemberEqualityComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>]
[KeyMemberComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>]
public partial struct NameStructSpanParsable
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

   public static ValidationError? Validate(ReadOnlySpan<char> value, IFormatProvider? provider, out NameStructSpanParsable item)
   {
      return value switch
      {
         "Value" => Validate("Value", provider, out item),
         _ => Validate(value.ToString(), provider, out item)
      };
   }

   public ReadOnlySpan<char> ToValue()
   {
      return _value;
   }
}

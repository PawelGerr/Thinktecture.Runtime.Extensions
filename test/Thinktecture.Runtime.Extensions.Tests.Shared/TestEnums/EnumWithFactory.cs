using System;
using System.ComponentModel.DataAnnotations;

namespace Thinktecture.Runtime.Tests.TestEnums;

[ValueObjectFactory<string>(UseForSerialization = SerializationFrameworks.All)]
public sealed partial class EnumWithFactory : IEnum<int>
{
   public static readonly EnumWithFactory Item1 = new(1);
   public static readonly EnumWithFactory Item2 = new(2);

   public static ValidationResult? Validate(string? value, IFormatProvider? provider, out EnumWithFactory? item)
   {
      switch (value)
      {
         case "=1=":
            item = Item1;
            return ValidationResult.Success;
         case "=2=":
            item = Item2;
            return ValidationResult.Success;
         default:
            item = null;
            return new ValidationResult($"Unknown item '{value}'");
      }
   }

   public string ToValue()
   {
      return $"={Key}=";
   }
}

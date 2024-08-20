using System;

namespace Thinktecture.Runtime.Tests.TestEnums;

[SmartEnum<int>]
[ValueObjectFactory<string>(UseForSerialization = SerializationFrameworks.All)]
public partial class EnumWithFactory
{
   public static readonly EnumWithFactory Item1 = new(1);
   public static readonly EnumWithFactory Item2 = new(2);

   public static ValidationError? Validate(string? value, IFormatProvider? provider, out EnumWithFactory? item)
   {
      switch (value)
      {
         case "=1=":
            item = Item1;
            return null;
         case "=2=":
            item = Item2;
            return null;
         default:
            item = null;
            return new ValidationError($"Unknown item '{value}'");
      }
   }

   public string ToValue()
   {
      return $"={Key}=";
   }
}

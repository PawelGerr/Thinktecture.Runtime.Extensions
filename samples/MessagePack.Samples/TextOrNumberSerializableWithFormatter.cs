using System;

namespace Thinktecture;

[Union<string, int>(T1IsNullableReferenceType = true,
                    T1Name = "Text",
                    T2Name = "Number",
                    SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
                    MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
[ObjectFactory<string>(UseForSerialization = SerializationFrameworks.All)]
public partial class TextOrNumberSerializableWithFormatter
{
   // For serialization (implementation of IConvertible<string>)
   public string ToValue()
   {
      return Switch(text: t => $"Text|{t}",
                    number: n => $"Number|{n}");
   }

   // For deserialization (implementation of IObjectFactory<TextOrNumberSerializableWithFormatter, string, ValidationError>)
   public static ValidationError? Validate(string? value, IFormatProvider? provider, out TextOrNumberSerializableWithFormatter? item)
   {
      if (String.IsNullOrWhiteSpace(value))
      {
         item = null;
         return null;
      }

      if (value.StartsWith("Text|", StringComparison.OrdinalIgnoreCase))
      {
         item = value.Substring(5);
         return null;
      }

      if (value.StartsWith("Number|", StringComparison.OrdinalIgnoreCase))
      {
         if (Int32.TryParse(value.Substring(7), out var number))
         {
            item = number;
            return null;
         }

         item = null;
         return new ValidationError("Invalid number format");
      }

      item = null;
      return new ValidationError("Invalid format");
   }
}

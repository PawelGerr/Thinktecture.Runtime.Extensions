using System;
using System.Text.Json.Serialization;
using Thinktecture.Text.Json.Serialization;

namespace Thinktecture.Unions;

[Union<string, int>(T1Name = "Text",
                    T2Name = "Number",
                    SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
                    MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
[ObjectFactory<string>(UseForSerialization = SerializationFrameworks.All)]
public partial class TextOrNumberSerializable
{
   // For serialization (implementation of IConvertible<string>)
   public string ToValue()
   {
      return Switch(text: t => $"Text|{t}",
                    number: n => $"Number|{n}");
   }

   // For deserialization (implementation of IObjectFactory<TextOrNumberSerializable, string, ValidationError>)
   public static ValidationError? Validate(string? value, IFormatProvider? provider, out TextOrNumberSerializable? item)
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

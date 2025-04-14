using System;
using System.Diagnostics.CodeAnalysis;
using MessagePack;

namespace Thinktecture;

[Union<string, int>(T1IsNullableReferenceType = true,
                    T1Name = "Text",
                    T2Name = "Number",
                    SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
                    MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
[ValueObjectFactory<string>(UseForSerialization = SerializationFrameworks.All)]
[MessagePackFormatter(typeof(Formatters.ValueObjectMessagePackFormatter<TextOrNumberSerializableWithFormatter, string, ValidationError>))] // Optional: otherwise ValueObjectMessageFormatterResolver is required
public partial class TextOrNumberSerializableWithFormatter :
   IValueObjectFactory<TextOrNumberSerializableWithFormatter, string, ValidationError>, // For deserialization
   IValueObjectConvertible<string>,                                                     // For serialization
   IParsable<TextOrNumberSerializableWithFormatter>                                     // For Minimal API and ASP.NET Core model binding validation
{
   // For serialization
   public string ToValue()
   {
      return Switch(text: t => $"Text|{t}",
                    number: n => $"Number|{n}");
   }

   // For deserialization
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

   public static TextOrNumberSerializableWithFormatter Parse(string s, IFormatProvider? provider)
   {
      var validationError = Validate(s, provider, out var result);

      if (validationError is null)
         return result!;

      throw new FormatException(validationError.Message);
   }

   public static bool TryParse(
      [NotNullWhen(true)] string? s,
      IFormatProvider? provider,
      [MaybeNullWhen(false)] out TextOrNumberSerializableWithFormatter result)
   {
      if (s is null)
      {
         result = null;
         return false;
      }

      var validationError = Validate(s, provider, out result!);
      return validationError is null;
   }
}

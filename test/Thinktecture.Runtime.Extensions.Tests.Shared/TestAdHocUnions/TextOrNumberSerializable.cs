using System;
using System.Text.Json.Serialization;
using Thinktecture.Text.Json.Serialization;

namespace Thinktecture.Runtime.Tests.TestAdHocUnions;

[Union<string, int>(T1Name = "Text", T2Name = "Number")]
[ObjectFactory<string>(UseForSerialization = SerializationFrameworks.All)]
[JsonConverter(typeof(ThinktectureJsonConverterFactory))]
public partial class TextOrNumberSerializable
{
   public string ToValue()
   {
      return Switch(text: t => $"Text|{t}",
                    number: n => $"Number|{n}");
   }

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

using System;

namespace Thinktecture.Runtime.Tests.Swashbuckle.Helpers;

// The object factory opts in to serialization for MessagePack only. Swashbuckle documents either the
// System.Text.Json or the Newtonsoft.Json wire format, so a factory flagged for neither must not influence the
// documented schema, not even when Swashbuckle.AspNetCore.Newtonsoft is registered.
[ComplexValueObject(DefaultStringComparison = StringComparison.OrdinalIgnoreCase)]
[ObjectFactory<string>(UseForSerialization = SerializationFrameworks.MessagePack)]
public partial class ComplexValueObjectWithMessagePackOnlyObjectFactory
{
   public string Property1 { get; }
   public string Property2 { get; }

   public static ValidationError? Validate(
      string? value,
      IFormatProvider? provider,
      out ComplexValueObjectWithMessagePackOnlyObjectFactory? item)
   {
      if (value is null)
      {
         item = null;
         return null;
      }

      var parts = value.Split('|');

      item = new(parts[0], parts[1]);
      return null;
   }

   public string ToValue()
   {
      return $"{Property1}|{Property2}";
   }
}

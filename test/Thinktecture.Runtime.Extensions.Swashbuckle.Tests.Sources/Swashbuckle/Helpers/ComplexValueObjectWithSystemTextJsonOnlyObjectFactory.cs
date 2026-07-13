using System;

namespace Thinktecture.Runtime.Tests.Swashbuckle.Helpers;

// The object factory opts in to serialization for System.Text.Json only (not Newtonsoft.Json).
// The OpenAPI schema filter must still treat the type as a string, because the configured
// System.Text.Json serializer uses the factory.
[ComplexValueObject(DefaultStringComparison = StringComparison.OrdinalIgnoreCase)]
[ObjectFactory<string>(UseForSerialization = SerializationFrameworks.SystemTextJson)]
public partial class ComplexValueObjectWithSystemTextJsonOnlyObjectFactory
{
   public string Property1 { get; }
   public string Property2 { get; }

   public static ValidationError? Validate(
      string? value,
      IFormatProvider? provider,
      out ComplexValueObjectWithSystemTextJsonOnlyObjectFactory? item)
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

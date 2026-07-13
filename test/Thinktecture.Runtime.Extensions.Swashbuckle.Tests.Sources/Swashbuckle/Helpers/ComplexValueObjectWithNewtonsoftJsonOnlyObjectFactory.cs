using System;

namespace Thinktecture.Runtime.Tests.Swashbuckle.Helpers;

// The object factory opts in to serialization for Newtonsoft.Json only (not System.Text.Json).
// Without Swashbuckle.AspNetCore.Newtonsoft registered, Swashbuckle documents the System.Text.Json
// wire format, which does not use this factory, so the schema filter must ignore it.
[ComplexValueObject(DefaultStringComparison = StringComparison.OrdinalIgnoreCase)]
[ObjectFactory<string>(UseForSerialization = SerializationFrameworks.NewtonsoftJson)]
public partial class ComplexValueObjectWithNewtonsoftJsonOnlyObjectFactory
{
   public string Property1 { get; }
   public string Property2 { get; }

   public static ValidationError? Validate(
      string? value,
      IFormatProvider? provider,
      out ComplexValueObjectWithNewtonsoftJsonOnlyObjectFactory? item)
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

using System;

namespace Thinktecture.Runtime.Tests.Swashbuckle.Helpers;

// Two object factories opt in to serialization for different JSON frameworks:
//  - the string factory for System.Text.Json,
//  - the int factory for Newtonsoft.Json (declared last).
// Swashbuckle documents the System.Text.Json wire format by default, so the schema filter must prefer the
// System.Text.Json factory ("string") and must not pick the last-declared Newtonsoft.Json factory ("integer").
[ComplexValueObject]
[ObjectFactory<string>(UseForSerialization = SerializationFrameworks.SystemTextJson)]
[ObjectFactory<int>(UseForSerialization = SerializationFrameworks.NewtonsoftJson)]
public partial class ComplexValueObjectWithFrameworkSpecificObjectFactories
{
   public string Value { get; }

   public static ValidationError? Validate(
      string? value,
      IFormatProvider? provider,
      out ComplexValueObjectWithFrameworkSpecificObjectFactories? item)
   {
      if (value is null)
      {
         item = null;
         return null;
      }

      item = new(value);
      return null;
   }

   public static ValidationError? Validate(
      int value,
      IFormatProvider? provider,
      out ComplexValueObjectWithFrameworkSpecificObjectFactories? item)
   {
      item = new(value.ToString());
      return null;
   }

   public string ToValue()
   {
      return Value;
   }

   int Thinktecture.IConvertible<int>.ToValue()
   {
      return Value.Length;
   }
}

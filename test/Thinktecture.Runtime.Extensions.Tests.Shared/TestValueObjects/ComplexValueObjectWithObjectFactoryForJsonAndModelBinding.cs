using System;

namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ComplexValueObject(DefaultStringComparison = StringComparison.OrdinalIgnoreCase)]
[ObjectFactory<string>(
   UseForSerialization = SerializationFrameworks.All,
   UseForModelBinding = true)]
public partial class ComplexValueObjectWithObjectFactoryForJsonAndModelBinding
{
   public string Property1 { get; }
   public string Property2 { get; }

   public static ValidationError? Validate(
      string? value,
      IFormatProvider? provider,
      out ComplexValueObjectWithObjectFactoryForJsonAndModelBinding? item)
   {
      if (value is null)
      {
         item = null;
         return new ValidationError("Value cannot be null.");
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

#nullable enable
using System;

namespace Thinktecture.Runtime.Tests.TestEntities;

[ObjectFactory<string>(UseWithEntityFramework = true)]
// ReSharper disable once InconsistentNaming
public partial class CustomObject_ObjectFactory
{
   public required string Property1 { get; init; }
   public required string Property2 { get; init; }

   public static ValidationError? Validate(
      string? value,
      IFormatProvider? provider,
      out CustomObject_ObjectFactory? item)
   {
      if (value is null)
      {
         item = null;
         return new ValidationError("Value cannot be null.");
      }

      var parts = value.Split('|');

      item = new()
             {
                Property1 = parts[0],
                Property2 = parts[1]
             };
      return null;
   }

   public string ToValue()
   {
      return $"{Property1}|{Property2}";
   }
}

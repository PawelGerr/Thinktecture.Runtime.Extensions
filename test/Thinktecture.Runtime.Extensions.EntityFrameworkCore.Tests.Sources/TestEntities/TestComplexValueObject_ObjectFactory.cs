using System;

#nullable enable

namespace Thinktecture.Runtime.Tests.TestEntities;

[ComplexValueObject(DefaultStringComparison = StringComparison.OrdinalIgnoreCase)]
[ObjectFactory<string>(UseWithEntityFramework = true)]
// ReSharper disable once InconsistentNaming
public partial class TestComplexValueObject_ObjectFactory
{
   public string Property1 { get; }
   public string Property2 { get; }

   public static ValidationError? Validate(
      string? value,
      IFormatProvider? provider,
      out TestComplexValueObject_ObjectFactory? item)
   {
      if (value is null)
      {
         item = null;
         return null;
      }

      var parts = value.Split('|');

      return Validate(parts[0], parts[1], out item);
   }

   public string ToValue()
   {
      return $"{Property1}|{Property2}";
   }
}

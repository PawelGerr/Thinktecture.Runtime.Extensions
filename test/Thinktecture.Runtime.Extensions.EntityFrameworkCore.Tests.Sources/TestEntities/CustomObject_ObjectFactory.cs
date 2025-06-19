#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;

namespace Thinktecture.Runtime.Tests.TestEntities;

[ObjectFactory<string>(UseWithEntityFramework = true)]
// ReSharper disable once InconsistentNaming
public partial class CustomObject_ObjectFactory
{
   public required string Property1 { get; init; }
   public required string Property2 { get; init; }

   [SetsRequiredMembers]
   public CustomObject_ObjectFactory(string property1, string property2)
   {
      Property1 = property1;
      Property2 = property2;
   }

   public static ValidationError? Validate(
      string? value,
      IFormatProvider? provider,
      out CustomObject_ObjectFactory? item)
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

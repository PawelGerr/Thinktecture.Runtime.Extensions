using System;

namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
[SmartEnum<int>]
[ObjectFactory<string>(
   UseForSerialization = SerializationFrameworks.All,
   UseForModelBinding = true)]
public partial class SmartEnum_Factory
{
   public static readonly SmartEnum_Factory Item1 = new(1);
   public static readonly SmartEnum_Factory Item2 = new(2);

   public static ValidationError? Validate(string? value, IFormatProvider? provider, out SmartEnum_Factory? item)
   {
      if (value is null)
      {
         item = null;
         return null;
      }

      switch (value)
      {
         case "=1=":
            item = Item1;
            return null;
         case "=2=":
            item = Item2;
            return null;
         default:
            item = null;
            return new ValidationError($"Unknown item '{value}'");
      }
   }

   public string ToValue()
   {
      return $"={Key}=";
   }
}

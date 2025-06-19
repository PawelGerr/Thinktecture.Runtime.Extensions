using System;

namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
[SmartEnum]
[ObjectFactory<string>(UseForModelBinding = true)]
public partial class SmartEnum_Keyless_ObjectFactory
{
   public static readonly SmartEnum_Keyless_ObjectFactory Item1 = new(1);
   public static readonly SmartEnum_Keyless_ObjectFactory Item2 = new(2);

   public int IntProperty { get; }

   public static ValidationError? Validate(string? value, IFormatProvider? provider, out SmartEnum_Keyless_ObjectFactory? item)
   {
      if (value is null)
      {
         item = null;
         return null;
      }

      switch (value)
      {
         case "1":
            item = Item1;
            return null;

         case "2":
            item = Item2;
            return null;

         default:
            item = null;
            return new ValidationError($"Unknown item '{value}'");
      }
   }
}

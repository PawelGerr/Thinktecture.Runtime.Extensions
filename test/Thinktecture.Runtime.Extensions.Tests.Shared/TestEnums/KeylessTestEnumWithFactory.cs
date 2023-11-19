using System;

namespace Thinktecture.Runtime.Tests.TestEnums;

[SmartEnum]
[ValueObjectFactory<string>]
public sealed partial class KeylessTestEnumWithFactory
{
   public static readonly KeylessTestEnumWithFactory Item1 = new(1);
   public static readonly KeylessTestEnumWithFactory Item2 = new(2);

   public int IntProperty { get; }

   public static ValidationError? Validate(string? value, IFormatProvider? provider, out KeylessTestEnumWithFactory? item)
   {
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

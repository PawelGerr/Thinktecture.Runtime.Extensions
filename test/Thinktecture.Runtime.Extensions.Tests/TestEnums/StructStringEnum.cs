using System;

namespace Thinktecture.TestEnums
{
   public readonly partial struct StructStringEnum : IValidatableEnum<string>
   {
      public static readonly StructStringEnum Item1 = new(String.Empty);
      public static readonly StructStringEnum Item2 = new("item2");
   }
}

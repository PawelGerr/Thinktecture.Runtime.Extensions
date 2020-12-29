using System;

namespace Thinktecture.TestEnums
{
   public readonly partial struct StructIntegerEnum : IValidatableEnum<int>
   {
      public static readonly StructIntegerEnum Item1 = new(1);
      public static readonly StructIntegerEnum Item2 = new(2);
   }
}

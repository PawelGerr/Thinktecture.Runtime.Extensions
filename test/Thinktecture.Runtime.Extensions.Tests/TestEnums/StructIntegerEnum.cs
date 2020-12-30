using System;

namespace Thinktecture.TestEnums
{
   public readonly partial struct StructIntegerEnum : IValidatableEnum<int>
   {
      public static readonly StructIntegerEnum Item1 = new(1, 42, 100);
      public static readonly StructIntegerEnum Item2 = new(2, 43, 200);

      public int Property1 => Field;

      public int Property2
      {
         get { return Field; }
      }

      public int Property3 { get; }
      public readonly int Field;
   }
}

using System;

#pragma warning disable CA1823, RCS1213 // Remove unused member declaration.
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local

namespace Thinktecture.TestEnums
{
   public partial class TestEnum : IEnum<String>
   {
      public static readonly TestEnum Item1 = new("item1");
      public static readonly TestEnum Item2 = new("item2");

      // the following members will lead to compiler error
      // protected static readonly TestEnum Item3 = new("item3");
      // internal static readonly TestEnum Item4 = new("item4");
      // private static readonly TestEnum Item5 = new("item5");

      public static readonly IntegerEnum Item6 = IntegerEnum.Item1;
      public static readonly IntegerEnum Item7 = IntegerEnum.Get(42);
   }
}

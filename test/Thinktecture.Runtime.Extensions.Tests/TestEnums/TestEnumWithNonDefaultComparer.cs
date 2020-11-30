using System;
using System.Collections.Generic;

namespace Thinktecture.TestEnums
{
   [EnumGeneration(KeyComparerProvidingMember = "_equalityComparer")]
   public partial class TestEnumWithNonDefaultComparer : IEnum<string>
   {
      private static readonly IEqualityComparer<string> _equalityComparer = StringComparer.Ordinal;

      public static readonly TestEnumWithNonDefaultComparer Item = new("item");
      public static readonly TestEnumWithNonDefaultComparer AnotherItem = new("Item");
   }
}

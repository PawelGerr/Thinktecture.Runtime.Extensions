using System;
using System.Collections.Generic;

namespace Thinktecture.Runtime.Tests.TestEnums;

public sealed partial class TestEnumWithNonDefaultComparer : IValidatableEnum<string>
{
   public static IEqualityComparer<string> KeyEqualityComparer => StringComparer.Ordinal;

   public static readonly TestEnumWithNonDefaultComparer Item = new("item");
   public static readonly TestEnumWithNonDefaultComparer AnotherItem = new("Item");
}

using System;
using System.Collections.Generic;

namespace Thinktecture.Runtime.Tests.TestEnums;

[EnumGeneration(KeyComparer = nameof(_equalityComparer))]
public partial class TestEnumWithNonDefaultComparer : IValidatableEnum<string>
{
   private static readonly IEqualityComparer<string> _equalityComparer = StringComparer.Ordinal;

   public static readonly TestEnumWithNonDefaultComparer Item = new("item");
   public static readonly TestEnumWithNonDefaultComparer AnotherItem = new("Item");
}
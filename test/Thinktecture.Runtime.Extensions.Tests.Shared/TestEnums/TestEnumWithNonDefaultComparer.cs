using System;
using System.Collections.Generic;

namespace Thinktecture.Runtime.Tests.TestEnums;

#if !NET7_0
[EnumGeneration(KeyComparer = nameof(_equalityComparer))]
#endif
public sealed partial class TestEnumWithNonDefaultComparer : IValidatableEnum<string>
{
#if NET7_0
   public static IEqualityComparer<string> KeyEqualityComparer => StringComparer.Ordinal;
#else
   private static readonly IEqualityComparer<string> _equalityComparer = StringComparer.Ordinal;
#endif

   public static readonly TestEnumWithNonDefaultComparer Item = new("item");
   public static readonly TestEnumWithNonDefaultComparer AnotherItem = new("Item");
}

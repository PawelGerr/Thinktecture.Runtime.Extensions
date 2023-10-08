using System;

namespace Thinktecture.Runtime.Tests.TestEnums;

[SmartEnum<string>(IsValidatable = true)]
public readonly partial struct StructStringEnum
{
   public static readonly StructStringEnum Item1 = new(String.Empty);
   public static readonly StructStringEnum Item2 = new("item2");
}

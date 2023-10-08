namespace Thinktecture.Runtime.Tests.TestEnums;

[SmartEnum<int>(IsValidatable = true)]
public readonly partial struct StructIntegerEnumWithZero
{
   public static readonly StructIntegerEnumWithZero Item0 = new(0);
   public static readonly StructIntegerEnumWithZero Item1 = new(1);
   public static readonly StructIntegerEnumWithZero Item2 = new(2);
}

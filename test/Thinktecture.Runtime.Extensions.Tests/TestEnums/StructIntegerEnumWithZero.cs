namespace Thinktecture.TestEnums
{
   public readonly partial struct StructIntegerEnumWithZero : IEnum<int>
   {
      public static readonly StructIntegerEnumWithZero Item0 = new(0);
      public static readonly StructIntegerEnumWithZero Item1 = new(1);
      public static readonly StructIntegerEnumWithZero Item2 = new(2);
   }
}

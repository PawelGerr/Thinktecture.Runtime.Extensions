namespace Thinktecture.CodeAnalysis.SmartEnums;

public class EnumTypeOnlyComparer : IEqualityComparer<SmartEnumDerivedTypes>
{
   public static readonly IEqualityComparer<SmartEnumDerivedTypes> Instance = new EnumTypeOnlyComparer();

   private EnumTypeOnlyComparer()
   {
   }

   public bool Equals(SmartEnumDerivedTypes? x, SmartEnumDerivedTypes? y)
   {
      if (x is null)
         return y is null;

      if (y is null)
         return false;

      return x.TypeFullyQualified == y.TypeFullyQualified;
   }

   public int GetHashCode(SmartEnumDerivedTypes obj)
   {
      return obj.TypeFullyQualified.GetHashCode();
   }
}

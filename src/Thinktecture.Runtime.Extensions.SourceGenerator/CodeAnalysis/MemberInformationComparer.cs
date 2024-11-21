namespace Thinktecture.CodeAnalysis;

public sealed class MemberInformationComparer : IEqualityComparer<IMemberInformation>
{
   public static readonly MemberInformationComparer Instance = new();

   public bool Equals(IMemberInformation? x, IMemberInformation? y)
   {
      if (x is null)
         return y is null;

      if (y is null)
         return false;

      return x.Name == y.Name
             && x.TypeFullyQualified == y.TypeFullyQualified
             && x.IsReferenceType == y.IsReferenceType
             && x.SpecialType == y.SpecialType;
   }

   public int GetHashCode(IMemberInformation obj)
   {
      unchecked
      {
         var hashCode = obj.Name.GetHashCode();
         hashCode = (hashCode * 397) ^ obj.TypeFullyQualified.GetHashCode();
         hashCode = (hashCode * 397) ^ obj.IsReferenceType.GetHashCode();
         hashCode = (hashCode * 397) ^ (int)obj.SpecialType;

         return hashCode;
      }
   }
}

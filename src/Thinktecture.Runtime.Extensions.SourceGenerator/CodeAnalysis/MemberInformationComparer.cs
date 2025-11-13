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
             && x.SpecialType == y.SpecialType
             && x.IsReferenceType == y.IsReferenceType
             && x.IsValueType == y.IsValueType
             && x.IsRecord == y.IsRecord
             && x.IsTypeParameter == y.IsTypeParameter
             && x.NullableAnnotation == y.NullableAnnotation
             && x.IsNullableStruct == y.IsNullableStruct;
   }

   public int GetHashCode(IMemberInformation obj)
   {
      unchecked
      {
         var hashCode = obj.Name.GetHashCode();
         hashCode = (hashCode * 397) ^ obj.TypeFullyQualified.GetHashCode();
         hashCode = (hashCode * 397) ^ obj.IsReferenceType.GetHashCode();
         hashCode = (hashCode * 397) ^ (int)obj.SpecialType;
         hashCode = (hashCode * 397) ^ obj.IsValueType.GetHashCode();
         hashCode = (hashCode * 397) ^ obj.IsRecord.GetHashCode();
         hashCode = (hashCode * 397) ^ obj.IsTypeParameter.GetHashCode();
         hashCode = (hashCode * 397) ^ (int)obj.NullableAnnotation;
         hashCode = (hashCode * 397) ^ obj.IsNullableStruct.GetHashCode();

         return hashCode;
      }
   }
}

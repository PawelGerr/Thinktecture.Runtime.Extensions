namespace Thinktecture.CodeAnalysis;

public sealed class ParsableMemberInformationComparer : IEqualityComparer<IParsableMemberInformation>
{
   public static readonly ParsableMemberInformationComparer Instance = new();

   public bool Equals(IParsableMemberInformation? x, IParsableMemberInformation? y)
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
             && x.IsNullableStruct == y.IsNullableStruct
             && x.IsParsable == y.IsParsable
             && x.IsSpanParsable == y.IsSpanParsable;
   }

   public int GetHashCode(IParsableMemberInformation obj)
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
         hashCode = (hashCode * 397) ^ obj.IsParsable.GetHashCode();
         hashCode = (hashCode * 397) ^ obj.IsSpanParsable.GetHashCode();

         return hashCode;
      }
   }
}

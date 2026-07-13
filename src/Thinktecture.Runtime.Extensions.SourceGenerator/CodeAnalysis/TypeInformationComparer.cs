namespace Thinktecture.CodeAnalysis;

public sealed class TypeInformationComparer : IEqualityComparer<ITypeInformation>
{
   public static readonly TypeInformationComparer Instance = new();

   public bool Equals(ITypeInformation? x, ITypeInformation? y)
   {
      if (x is null)
         return y is null;

      if (y is null)
         return false;

      return x.TypeFullyQualified == y.TypeFullyQualified
             && x.IsReferenceType == y.IsReferenceType
             && x.ContainingTypes.SequenceEqual(y.ContainingTypes);
   }

   public int GetHashCode(ITypeInformation obj)
   {
      unchecked
      {
         var hashCode = obj.TypeFullyQualified.GetHashCode();
         hashCode = (hashCode * 397) ^ obj.IsReferenceType.GetHashCode();
         hashCode = (hashCode * 397) ^ obj.ContainingTypes.ComputeHashCode();

         return hashCode;
      }
   }
}

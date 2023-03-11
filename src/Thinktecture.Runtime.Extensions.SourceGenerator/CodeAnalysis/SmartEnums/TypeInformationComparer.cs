namespace Thinktecture.CodeAnalysis.SmartEnums;

public sealed class TypeInformationComparer : IEqualityComparer<ITypeInformation>
{
   public static readonly IEqualityComparer<ITypeInformation> Instance = new TypeInformationComparer();

   public bool Equals(ITypeInformation? x, ITypeInformation? y)
   {
      if (x is null)
         return y is null;

      if (y is null)
         return false;

      return x.TypeMinimallyQualified == y.TypeMinimallyQualified
             && x.IsReferenceType == y.IsReferenceType;
   }

   public int GetHashCode(ITypeInformation obj)
   {
      unchecked
      {
         var hashCode = obj.TypeMinimallyQualified.GetHashCode();
         hashCode = (hashCode * 397) ^ obj.IsReferenceType.GetHashCode();

         return hashCode;
      }
   }
}

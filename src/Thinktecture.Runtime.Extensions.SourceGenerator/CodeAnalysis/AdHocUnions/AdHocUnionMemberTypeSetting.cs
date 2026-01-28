namespace Thinktecture.CodeAnalysis.AdHocUnions;

public readonly struct AdHocUnionMemberTypeSetting : IEquatable<AdHocUnionMemberTypeSetting>, IHashCodeComputable
{
   public bool IsNullableReferenceType { get; }
   public bool IsStateless { get; }
   public string? Name { get; }

   public AdHocUnionMemberTypeSetting(
      bool isNullableReferenceType,
      bool isStateless,
      string? name)
   {
      IsNullableReferenceType = isNullableReferenceType;
      IsStateless = isStateless;
      Name = name;
   }

   public override bool Equals(object? obj)
   {
      return obj is AdHocUnionMemberTypeSetting other && Equals(other);
   }

   public bool Equals(AdHocUnionMemberTypeSetting other)
   {
      return IsNullableReferenceType == other.IsNullableReferenceType
             && IsStateless == other.IsStateless
             && Name == other.Name;
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = IsNullableReferenceType.GetHashCode();
         hashCode = (hashCode * 397) ^ IsStateless.GetHashCode();
         hashCode = (hashCode * 397) ^ (Name?.GetHashCode() ?? 0);
         return hashCode;
      }
   }

   public static bool operator ==(AdHocUnionMemberTypeSetting left, AdHocUnionMemberTypeSetting right)
   {
      return left.Equals(right);
   }

   public static bool operator !=(AdHocUnionMemberTypeSetting left, AdHocUnionMemberTypeSetting right)
   {
      return !(left == right);
   }
}

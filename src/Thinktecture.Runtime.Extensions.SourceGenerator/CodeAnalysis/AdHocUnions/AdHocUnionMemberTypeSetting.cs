namespace Thinktecture.CodeAnalysis.AdHocUnions;

public readonly struct AdHocUnionMemberTypeSetting : IEquatable<AdHocUnionMemberTypeSetting>, IHashCodeComputable
{
   public bool IsNullableReferenceType { get; }
   public string? Name { get; }

   public AdHocUnionMemberTypeSetting(
      bool isNullableReferenceType,
      string? name)
   {
      IsNullableReferenceType = isNullableReferenceType;
      Name = name;
   }

   public override bool Equals(object? obj)
   {
      return obj is AdHocUnionMemberTypeSetting other && Equals(other);
   }

   public bool Equals(AdHocUnionMemberTypeSetting other)
   {
      return IsNullableReferenceType == other.IsNullableReferenceType
             && Name == other.Name;
   }

   public override int GetHashCode()
   {
      unchecked
      {
         return (IsNullableReferenceType.GetHashCode() * 397) ^ (Name?.GetHashCode() ?? 0);
      }
   }
}

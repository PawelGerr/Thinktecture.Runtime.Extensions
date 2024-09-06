namespace Thinktecture.CodeAnalysis.DiscriminatedUnions;

public readonly struct MemberTypeSetting : IEquatable<MemberTypeSetting>, IHashCodeComputable
{
   public bool IsNullableReferenceType { get; }
   public string? Name { get; }

   public MemberTypeSetting(
      bool isNullableReferenceType,
      string? name)
   {
      IsNullableReferenceType = isNullableReferenceType;
      Name = name;
   }

   public override bool Equals(object? obj)
   {
      return obj is MemberTypeSetting other && Equals(other);
   }

   public bool Equals(MemberTypeSetting other)
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

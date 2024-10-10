namespace Thinktecture.CodeAnalysis.DiscriminatedUnions;

public sealed class MemberTypeState : IEquatable<MemberTypeState>, IMemberInformation, ITypeMinimallyQualified, IHashCodeComputable
{
   public string TypeFullyQualified { get; }
   public string TypeMinimallyQualified { get; }
   public string Name { get; }
   public bool IsReferenceType { get; }
   public NullableAnnotation NullableAnnotation { get; }
   public bool IsNullableStruct { get; }
   public SpecialType SpecialType { get; }

   public string ArgumentName { get; }
   public MemberTypeSetting Setting { get; }

   public MemberTypeState(
      INamedTypeSymbol type,
      ITypedMemberState typeState,
      MemberTypeSetting setting)
   {
      Name = setting.Name ?? (typeState.IsNullableStruct ? $"Nullable{type.TypeArguments[0].Name}" : type.Name);
      TypeFullyQualified = typeState.TypeFullyQualified;
      TypeMinimallyQualified = typeState.TypeMinimallyQualified;
      IsReferenceType = typeState.IsReferenceType;
      NullableAnnotation = typeState.NullableAnnotation;
      IsNullableStruct = typeState.IsNullableStruct;
      SpecialType = typeState.SpecialType;

      ArgumentName = Name.MakeArgumentName();
      Setting = setting;
   }

   public override bool Equals(object? obj)
   {
      return obj is MemberTypeState other && Equals(other);
   }

   public bool Equals(MemberTypeState? other)
   {
      if (other is null)
         return false;
      if (ReferenceEquals(this, other))
         return true;

      return TypeFullyQualified == other.TypeFullyQualified
             && IsReferenceType == other.IsReferenceType
             && SpecialType == other.SpecialType
             && Setting.Equals(other.Setting);
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = TypeFullyQualified.GetHashCode();
         hashCode = (hashCode * 397) ^ IsReferenceType.GetHashCode();
         hashCode = (hashCode * 397) ^ (int)SpecialType;
         hashCode = (hashCode * 397) ^ Setting.GetHashCode();

         return hashCode;
      }
   }
}

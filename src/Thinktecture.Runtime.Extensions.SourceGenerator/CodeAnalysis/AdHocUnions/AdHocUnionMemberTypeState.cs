namespace Thinktecture.CodeAnalysis.AdHocUnions;

public sealed class AdHocUnionMemberTypeState
   : IEquatable<AdHocUnionMemberTypeState>,
     IMemberInformation,
     ITypeMinimallyQualified,
     IHashCodeComputable
{
   public string TypeFullyQualified { get; }
   public string TypeMinimallyQualified { get; }
   public string Name { get; }
   public bool IsReferenceType { get; }
   public bool IsStruct { get; }
   public NullableAnnotation NullableAnnotation { get; }
   public bool IsNullableStruct { get; }
   public SpecialType SpecialType { get; }
   public bool IsInterface { get; }
   public int TypeDuplicateCounter { get; }

   public ArgumentName ArgumentName { get; }
   public BackingFieldName BackingFieldName { get; }
   public AdHocUnionMemberTypeSetting Setting { get; }

   public bool IsRecord => false;
   public bool IsTypeParameter => false;

   public AdHocUnionMemberTypeState(
      string name,
      string defaultName,
      int typeDuplicateCounter,
      ITypedMemberState typeState,
      AdHocUnionMemberTypeSetting setting)
   {
      Name = name;
      ArgumentName = ArgumentName.Create(Name);

      var backingFieldName = typeDuplicateCounter == 0 ? Name : defaultName;
      BackingFieldName = BackingFieldName.Create(backingFieldName, Name);

      TypeDuplicateCounter = typeDuplicateCounter;
      TypeFullyQualified = typeState.TypeFullyQualified;
      TypeMinimallyQualified = typeState.TypeMinimallyQualified;
      IsReferenceType = typeState.IsReferenceType;
      IsStruct = typeState.IsStruct;
      NullableAnnotation = typeState.NullableAnnotation;
      IsNullableStruct = typeState.IsNullableStruct;
      SpecialType = typeState.SpecialType;
      IsInterface = typeState.TypeKind == TypeKind.Interface;
      Setting = setting;
   }

   public override bool Equals(object? obj)
   {
      return obj is AdHocUnionMemberTypeState other && Equals(other);
   }

   public bool Equals(AdHocUnionMemberTypeState? other)
   {
      if (other is null)
         return false;
      if (ReferenceEquals(this, other))
         return true;

      return TypeFullyQualified == other.TypeFullyQualified
             && IsReferenceType == other.IsReferenceType
             && IsStruct == other.IsStruct
             && SpecialType == other.SpecialType
             && IsInterface == other.IsInterface
             && TypeDuplicateCounter == other.TypeDuplicateCounter
             && Setting.Equals(other.Setting);
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = TypeFullyQualified.GetHashCode();
         hashCode = (hashCode * 397) ^ IsReferenceType.GetHashCode();
         hashCode = (hashCode * 397) ^ IsStruct.GetHashCode();
         hashCode = (hashCode * 397) ^ (int)SpecialType;
         hashCode = (hashCode * 397) ^ IsInterface.GetHashCode();
         hashCode = (hashCode * 397) ^ TypeDuplicateCounter;
         hashCode = (hashCode * 397) ^ Setting.GetHashCode();

         return hashCode;
      }
   }
}

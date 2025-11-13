namespace Thinktecture.CodeAnalysis.AdHocUnions;

public sealed class AdHocUnionSourceGenState
   : ITypeInformation,
     IEquatable<AdHocUnionSourceGenState>
{
   public string? Namespace { get; }
   public string Name { get; }
   public ImmutableArray<ContainingTypeState> ContainingTypes { get; }

   public string TypeFullyQualified { get; }
   public string TypeMinimallyQualified { get; }
   public bool IsReferenceType { get; }
   public bool IsValueType { get; }
   public bool IsRefStruct { get; }
   public bool IsEqualWithReferenceEquality => false;

   public bool IsNullableStruct => false;
   public NullableAnnotation NullableAnnotation => NullableAnnotation.NotAnnotated;
   public bool IsRecord => false;
   public bool IsTypeParameter => false;
   public bool DisallowsDefaultValue => true;
   public int NumberOfGenerics => 0;

   public ImmutableArray<AdHocUnionMemberTypeState> MemberTypes { get; }
   public AdHocUnionSettings Settings { get; }
   public AttributeInfo AttributeInfo { get; }

   public AdHocUnionSourceGenState(
      INamedTypeSymbol type,
      ImmutableArray<AdHocUnionMemberTypeState> memberTypes,
      AdHocUnionSettings settings,
      AttributeInfo attributeInfo)
   {
      MemberTypes = memberTypes;
      Settings = settings;
      AttributeInfo = attributeInfo;
      Name = type.Name;
      Namespace = type.ContainingNamespace?.IsGlobalNamespace == true ? null : type.ContainingNamespace?.ToString();
      ContainingTypes = type.GetContainingTypes();
      TypeFullyQualified = type.ToFullyQualifiedDisplayString();
      TypeMinimallyQualified = type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
      IsReferenceType = type.IsReferenceType;
      IsValueType = type.IsValueType;
      IsRefStruct = type is { IsRefLikeType: true, IsReferenceType: false };
   }

   public override bool Equals(object? obj)
   {
      return obj is AdHocUnionSourceGenState other && Equals(other);
   }

   public bool Equals(AdHocUnionSourceGenState? other)
   {
      if (other is null)
         return false;
      if (ReferenceEquals(this, other))
         return true;

      return TypeFullyQualified == other.TypeFullyQualified
             && IsReferenceType == other.IsReferenceType
             && IsValueType == other.IsValueType
             && IsRefStruct == other.IsRefStruct
             && Settings.Equals(other.Settings)
             && AttributeInfo.Equals(other.AttributeInfo)
             && MemberTypes.SequenceEqual(other.MemberTypes)
             && ContainingTypes.SequenceEqual(other.ContainingTypes);
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = TypeFullyQualified.GetHashCode();
         hashCode = (hashCode * 397) ^ IsReferenceType.GetHashCode();
         hashCode = (hashCode * 397) ^ IsValueType.GetHashCode();
         hashCode = (hashCode * 397) ^ IsRefStruct.GetHashCode();
         hashCode = (hashCode * 397) ^ Settings.GetHashCode();
         hashCode = (hashCode * 397) ^ AttributeInfo.GetHashCode();
         hashCode = (hashCode * 397) ^ MemberTypes.ComputeHashCode();
         hashCode = (hashCode * 397) ^ ContainingTypes.ComputeHashCode();

         return hashCode;
      }
   }
}

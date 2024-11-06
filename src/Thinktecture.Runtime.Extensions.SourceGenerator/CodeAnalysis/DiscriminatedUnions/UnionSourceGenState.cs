namespace Thinktecture.CodeAnalysis.DiscriminatedUnions;

public sealed class UnionSourceGenState : ITypeInformation, IEquatable<UnionSourceGenState>
{
   public string? Namespace { get; }
   public string Name { get; }
   public IReadOnlyList<ContainingTypeState> ContainingTypes { get; }
   public string TypeFullyQualified { get; }
   public string TypeMinimallyQualified { get; }
   public bool IsReferenceType { get; }
   public NullableAnnotation NullableAnnotation { get; }
   public bool IsNullableStruct { get; }
   public bool IsRefStruct { get; }
   public bool IsEqualWithReferenceEquality => false;

   public IReadOnlyList<MemberTypeState> MemberTypes { get; }
   public UnionSettings Settings { get; }

   public UnionSourceGenState(
      INamedTypeSymbol type,
      IReadOnlyList<MemberTypeState> memberTypes,
      UnionSettings settings)
   {
      MemberTypes = memberTypes;
      Settings = settings;
      Name = type.Name;
      Namespace = type.ContainingNamespace?.IsGlobalNamespace == true ? null : type.ContainingNamespace?.ToString();
      ContainingTypes = type.GetContainingTypes();
      TypeFullyQualified = type.ToFullyQualifiedDisplayString();
      TypeMinimallyQualified = type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
      IsReferenceType = type.IsReferenceType;
      NullableAnnotation = type.NullableAnnotation;
      IsNullableStruct = type.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T;
      IsRefStruct = type is { IsRefLikeType: true, IsReferenceType: false };
   }

   public override bool Equals(object? obj)
   {
      return obj is UnionSourceGenState other && Equals(other);
   }

   public bool Equals(UnionSourceGenState? other)
   {
      if (other is null)
         return false;
      if (ReferenceEquals(this, other))
         return true;

      return TypeFullyQualified == other.TypeFullyQualified
             && IsReferenceType == other.IsReferenceType
             && IsRefStruct == other.IsRefStruct
             && Settings.Equals(other.Settings)
             && MemberTypes.SequenceEqual(other.MemberTypes)
             && ContainingTypes.SequenceEqual(other.ContainingTypes);
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = TypeFullyQualified.GetHashCode();
         hashCode = (hashCode * 397) ^ IsReferenceType.GetHashCode();
         hashCode = (hashCode * 397) ^ IsRefStruct.GetHashCode();
         hashCode = (hashCode * 397) ^ Settings.GetHashCode();
         hashCode = (hashCode * 397) ^ MemberTypes.ComputeHashCode();
         hashCode = (hashCode * 397) ^ ContainingTypes.ComputeHashCode();

         return hashCode;
      }
   }
}

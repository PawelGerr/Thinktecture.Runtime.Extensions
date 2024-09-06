namespace Thinktecture.CodeAnalysis.DiscriminatedUnions;

public sealed class UnionSourceGenState : ITypeInformation, IEquatable<UnionSourceGenState>
{
   public string? Namespace { get; }
   public string Name { get; }
   public string TypeFullyQualified { get; }
   public string TypeMinimallyQualified { get; }
   public bool IsReferenceType { get; }
   public NullableAnnotation NullableAnnotation { get; set; }
   public bool IsNullableStruct { get; set; }
   public bool IsEqualWithReferenceEquality => false;

   public ImmutableArray<MemberTypeState> MemberTypes { get; }
   public UnionSettings Settings { get; }

   public UnionSourceGenState(
      INamedTypeSymbol type,
      ImmutableArray<MemberTypeState> memberTypes,
      UnionSettings settings)
   {
      MemberTypes = memberTypes;
      Settings = settings;
      Name = type.Name;
      Namespace = type.ContainingNamespace?.IsGlobalNamespace == true ? null : type.ContainingNamespace?.ToString();
      TypeFullyQualified = type.ToFullyQualifiedDisplayString();
      TypeMinimallyQualified = type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
      IsReferenceType = type.IsReferenceType;
      NullableAnnotation = type.NullableAnnotation;
      IsNullableStruct = type.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T;
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
             && Settings.Equals(other.Settings)
             && MemberTypes.SequenceEqual(other.MemberTypes);
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = TypeFullyQualified.GetHashCode();
         hashCode = (hashCode * 397) ^ IsReferenceType.GetHashCode();
         hashCode = (hashCode * 397) ^ Settings.GetHashCode();
         hashCode = (hashCode * 397) ^ MemberTypes.ComputeHashCode();

         return hashCode;
      }
   }
}

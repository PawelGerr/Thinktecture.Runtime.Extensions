namespace Thinktecture.CodeAnalysis.RegularUnions;

public sealed class RegularUnionSourceGenState
   : IEquatable<RegularUnionSourceGenState>,
     ITypeFullyQualified,
     ITypeKindInformation,
     IHasGenerics,
     INamespaceAndName,
     IHashCodeComputable
{
   public string? Namespace { get; }
   public string Name { get; }
   public string TypeFullyQualified { get; }
   public string TypeDefinitionFullyQualified { get; }
   public bool IsRecord { get; }
   public bool HasNonDefaultConstructor { get; }

   public bool IsTypeParameter => false;
   public bool IsReferenceType => true;
   public bool IsStruct => false;

   public IReadOnlyList<GenericTypeParameterState> GenericParameters { get; }
   public IReadOnlyList<ContainingTypeState> ContainingTypes { get; }
   public IReadOnlyList<RegularUnionTypeMemberState> TypeMembers { get; }
   public RegularUnionSettings Settings { get; }

   public int NumberOfGenerics => GenericParameters.Count;

   public RegularUnionSourceGenState(
      INamedTypeSymbol type,
      IReadOnlyList<RegularUnionTypeMemberState> typeMembers,
      RegularUnionSettings settings)
   {
      Name = type.Name;
      Namespace = type.ContainingNamespace?.IsGlobalNamespace == true ? null : type.ContainingNamespace?.ToString();
      TypeFullyQualified = type.ToFullyQualifiedDisplayString();
      TypeDefinitionFullyQualified = type.GetGenericTypeDefinition().ToFullyQualifiedDisplayString();
      HasNonDefaultConstructor = !type.Constructors.IsDefaultOrEmpty && type.Constructors.Any(c => !c.IsImplicitlyDeclared);
      ContainingTypes = type.GetContainingTypes();
      GenericParameters = type.GetGenericTypeParameters();

      IsRecord = type.IsRecord;
      TypeMembers = typeMembers;
      Settings = settings;
   }

   public override bool Equals(object? obj)
   {
      return Equals(obj as RegularUnionSourceGenState);
   }

   public bool Equals(RegularUnionSourceGenState? other)
   {
      if (other is null)
         return false;
      if (ReferenceEquals(this, other))
         return true;

      return TypeFullyQualified == other.TypeFullyQualified
             && IsRecord == other.IsRecord
             && HasNonDefaultConstructor == other.HasNonDefaultConstructor
             && Settings.Equals(other.Settings)
             && GenericParameters.SequenceEqual(other.GenericParameters)
             && ContainingTypes.SequenceEqual(other.ContainingTypes)
             && TypeMembers.SequenceEqual(other.TypeMembers);
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = TypeFullyQualified.GetHashCode();
         hashCode = (hashCode * 397) ^ IsRecord.GetHashCode();
         hashCode = (hashCode * 397) ^ HasNonDefaultConstructor.GetHashCode();
         hashCode = (hashCode * 397) ^ Settings.GetHashCode();
         hashCode = (hashCode * 397) ^ GenericParameters.ComputeHashCode();
         hashCode = (hashCode * 397) ^ ContainingTypes.ComputeHashCode();
         hashCode = (hashCode * 397) ^ TypeMembers.ComputeHashCode();

         return hashCode;
      }
   }
}

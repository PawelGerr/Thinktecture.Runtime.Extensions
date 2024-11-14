namespace Thinktecture.CodeAnalysis.Unions;

public class UnionSourceGenState : IEquatable<UnionSourceGenState>, ITypeFullyQualified, INamespaceAndName, IHashCodeComputable
{
   public string? Namespace { get; }
   public string Name { get; }
   public string TypeFullyQualified { get; }
   public bool IsRecord { get; }
   public bool HasNonDefaultConstructor { get; }

   public IReadOnlyList<string> GenericsFullyQualified { get; }
   public IReadOnlyList<ContainingTypeState> ContainingTypes { get; }
   public IReadOnlyList<UnionTypeMemberState> TypeMembers { get; }
   public UnionSettings Settings { get; }

   public UnionSourceGenState(
      INamedTypeSymbol type,
      IReadOnlyList<UnionTypeMemberState> typeMembers,
      UnionSettings settings)
   {
      Name = type.Name;
      Namespace = type.ContainingNamespace?.IsGlobalNamespace == true ? null : type.ContainingNamespace?.ToString();
      HasNonDefaultConstructor = !type.Constructors.IsDefaultOrEmpty && type.Constructors.Any(c => !c.IsImplicitlyDeclared);
      ContainingTypes = type.GetContainingTypes();
      GenericsFullyQualified = type.TypeArguments.Length == 0
                                  ? []
                                  : type.TypeArguments.Select(t => t.ToFullyQualifiedDisplayString()).ToList();

      if (type is { IsGenericType: true, IsUnboundGenericType: true })
      {
         TypeFullyQualified = type.OriginalDefinition.ToFullyQualifiedDisplayString();
      }
      else
      {
         TypeFullyQualified = type.ToFullyQualifiedDisplayString();
      }

      IsRecord = type.IsRecord;
      TypeMembers = typeMembers;
      Settings = settings;
   }

   public bool Equals(UnionSourceGenState? other)
   {
      if (other is null)
         return false;
      if (ReferenceEquals(this, other))
         return true;

      return TypeFullyQualified == other.TypeFullyQualified
             && IsRecord == other.IsRecord
             && HasNonDefaultConstructor == other.HasNonDefaultConstructor
             && Settings.Equals(other.Settings)
             && GenericsFullyQualified.SequenceEqual(other.GenericsFullyQualified)
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
         hashCode = (hashCode * 397) ^ GenericsFullyQualified.ComputeHashCode();
         hashCode = (hashCode * 397) ^ ContainingTypes.ComputeHashCode();
         hashCode = (hashCode * 397) ^ TypeMembers.ComputeHashCode();

         return hashCode;
      }
   }
}

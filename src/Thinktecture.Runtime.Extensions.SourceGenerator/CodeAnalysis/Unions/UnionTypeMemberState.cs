namespace Thinktecture.CodeAnalysis.Unions;

public class UnionTypeMemberState : IEquatable<UnionTypeMemberState>, ITypeFullyQualified, IHashCodeComputable
{
   public string TypeFullyQualified { get; }
   public string TypeDefinitionFullyQualified { get; }
   public string Name { get; }
   public bool IsAbstract { get; }
   public bool IsInterface { get; }
   public string BaseTypeFullyQualified { get; }
   public string BaseTypeDefinitionFullyQualified { get; }
   public IReadOnlyList<DefaultMemberState> UniqueSingleArgumentConstructors { get; }
   public IReadOnlyList<ContainingTypeState> ContainingTypes { get; }

   public UnionTypeMemberState(
      INamedTypeSymbol type,
      INamedTypeSymbol typeDefinition,
      IReadOnlyList<DefaultMemberState> uniqueSingleArgumentConstructors)
   {
      if (type.BaseType is null)
         throw new InvalidOperationException($"Inner union type ''{TypeFullyQualified} must have a base type.");

      Name = type.Name;
      TypeFullyQualified = type.ToFullyQualifiedDisplayString();
      TypeDefinitionFullyQualified = typeDefinition.ToFullyQualifiedDisplayString();
      BaseTypeFullyQualified = type.BaseType.ToFullyQualifiedDisplayString();
      BaseTypeDefinitionFullyQualified = type.BaseType.GetGenericTypeDefinition().ToFullyQualifiedDisplayString();
      IsAbstract = type.IsAbstract;
      IsInterface = type.TypeKind == TypeKind.Interface;
      UniqueSingleArgumentConstructors = uniqueSingleArgumentConstructors;

      ContainingTypes = type.GetContainingTypes();
   }

   public bool Equals(UnionTypeMemberState? other)
   {
      if (other is null)
         return false;
      if (ReferenceEquals(this, other))
         return true;

      return TypeFullyQualified == other.TypeFullyQualified
             && BaseTypeFullyQualified == other.BaseTypeFullyQualified
             && IsAbstract == other.IsAbstract
             && IsInterface == other.IsInterface
             && UniqueSingleArgumentConstructors.SequenceEqual(other.UniqueSingleArgumentConstructors)
             && ContainingTypes.SequenceEqual(other.ContainingTypes);
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = TypeFullyQualified.GetHashCode();
         hashCode = (hashCode * 397) ^ BaseTypeFullyQualified.GetHashCode();
         hashCode = (hashCode * 397) ^ IsAbstract.GetHashCode();
         hashCode = (hashCode * 397) ^ IsInterface.GetHashCode();
         hashCode = (hashCode * 397) ^ UniqueSingleArgumentConstructors.ComputeHashCode();
         hashCode = (hashCode * 397) ^ ContainingTypes.ComputeHashCode();

         return hashCode;
      }
   }
}

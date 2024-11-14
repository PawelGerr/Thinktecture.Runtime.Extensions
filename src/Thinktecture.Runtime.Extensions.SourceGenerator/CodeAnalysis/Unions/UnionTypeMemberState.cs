namespace Thinktecture.CodeAnalysis.Unions;

public class UnionTypeMemberState : IEquatable<UnionTypeMemberState>, ITypeFullyQualified, IHashCodeComputable
{
   public string TypeFullyQualified { get; }
   public string Name { get; }
   public bool IsAbstract { get; }
   public string BaseTypeFullyQualified { get; }
   public IReadOnlyList<ContainingTypeState> ContainingTypes { get; }

   public UnionTypeMemberState(
      INamedTypeSymbol type)
   {
      Name = type.Name;
      TypeFullyQualified = type.ToFullyQualifiedDisplayString();
      IsAbstract = type.IsAbstract;

      if (type is { IsGenericType: true, IsUnboundGenericType: true })
      {
         TypeFullyQualified = type.OriginalDefinition.ToFullyQualifiedDisplayString();
         BaseTypeFullyQualified = type.OriginalDefinition.BaseType?.ToFullyQualifiedDisplayString()
                                  ?? throw new InvalidOperationException($"Inner union type ''{TypeFullyQualified} must have a base type.");
      }
      else
      {
         TypeFullyQualified = type.ToFullyQualifiedDisplayString();
         BaseTypeFullyQualified = type.BaseType?.ToFullyQualifiedDisplayString()
                                  ?? throw new InvalidOperationException($"Inner union type ''{TypeFullyQualified} must have a base type.");
      }

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
             && ContainingTypes.SequenceEqual(other.ContainingTypes);
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = TypeFullyQualified.GetHashCode();
         hashCode = (hashCode * 397) ^ BaseTypeFullyQualified.GetHashCode();
         hashCode = (hashCode * 397) ^ IsAbstract.GetHashCode();
         hashCode = (hashCode * 397) ^ ContainingTypes.ComputeHashCode();

         return hashCode;
      }
   }
}

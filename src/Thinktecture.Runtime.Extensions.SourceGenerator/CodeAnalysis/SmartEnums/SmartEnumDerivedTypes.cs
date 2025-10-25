namespace Thinktecture.CodeAnalysis.SmartEnums;

public sealed class SmartEnumDerivedTypes : INamespaceAndName, ITypeFullyQualified, IEquatable<SmartEnumDerivedTypes>
{
   public string? Namespace { get; }
   public string Name { get; }
   public string TypeFullyQualified { get; }
   public bool IsReferenceType { get; }
   public IReadOnlyList<ContainingTypeState> ContainingTypes { get; }
   public IReadOnlyList<string> DerivedTypesFullyQualified { get; }
   public int NumberOfGenerics => 0;

   public SmartEnumDerivedTypes(
      string? ns,
      string name,
      string typeFullyQualified,
      bool isReferenceType,
      IReadOnlyList<ContainingTypeState> containingTypes,
      IReadOnlyList<string> derivedTypesFullyQualified)
   {
      Namespace = ns;
      Name = name;
      TypeFullyQualified = typeFullyQualified;
      IsReferenceType = isReferenceType;
      ContainingTypes = containingTypes;
      DerivedTypesFullyQualified = derivedTypesFullyQualified;
   }

   public bool Equals(SmartEnumDerivedTypes? other)
   {
      if (other is null)
         return false;

      if (ReferenceEquals(this, other))
         return true;

      return Namespace == other.Namespace
             && Name == other.Name
             && TypeFullyQualified == other.TypeFullyQualified
             && IsReferenceType == other.IsReferenceType
             && DerivedTypesFullyQualified.SequenceEqual(other.DerivedTypesFullyQualified)
             && ContainingTypes.SequenceEqual(other.ContainingTypes);
   }

   public override bool Equals(object? obj)
   {
      return obj is SmartEnumDerivedTypes derivedTypes && Equals(derivedTypes);
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = Namespace != null ? Namespace.GetHashCode() : 0;
         hashCode = (hashCode * 397) ^ Name.GetHashCode();
         hashCode = (hashCode * 397) ^ TypeFullyQualified.GetHashCode();
         hashCode = (hashCode * 397) ^ IsReferenceType.GetHashCode();
         hashCode = (hashCode * 397) ^ DerivedTypesFullyQualified.ComputeHashCode(StringComparer.Ordinal);
         hashCode = (hashCode * 397) ^ ContainingTypes.ComputeHashCode();

         return hashCode;
      }
   }
}

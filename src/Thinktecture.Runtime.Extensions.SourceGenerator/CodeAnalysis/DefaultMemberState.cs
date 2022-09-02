using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

public class DefaultMemberState : IMemberState, IEquatable<DefaultMemberState>
{
   private readonly ITypeSymbol _type;

   public string Name { get; }
   public string ArgumentName { get; }

   public string TypeFullyQualified { get; }
   public string TypeFullyQualifiedWithNullability { get; }
   public string TypeMinimallyQualified { get; }

   public bool IsStatic { get; }
   public SpecialType SpecialType => _type.SpecialType;
   public bool IsReferenceType => _type.IsReferenceType;

   public DefaultMemberState(string name, ITypeSymbol type, string argumentName, bool isStatic)
   {
      _type = type;

      Name = name;
      ArgumentName = argumentName;

      TypeFullyQualified = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
      TypeFullyQualifiedWithNullability = type.IsReferenceType && type.NullableAnnotation == NullableAnnotation.Annotated ? $"{TypeFullyQualified}?" : TypeFullyQualified;
      TypeMinimallyQualified = type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

      IsStatic = isStatic;
   }

   public override bool Equals(object? obj)
   {
      return obj is DefaultMemberState other && Equals(other);
   }

   public bool Equals(IMemberState? obj)
   {
      return obj is DefaultMemberState other && Equals(other);
   }

   public bool Equals(DefaultMemberState? other)
   {
      if (ReferenceEquals(null, other))
         return false;
      if (ReferenceEquals(this, other))
         return true;

      return Name == other.Name
             && ArgumentName == other.ArgumentName
             && TypeFullyQualifiedWithNullability == other.TypeFullyQualifiedWithNullability
             && IsStatic == other.IsStatic
             && SpecialType == other.SpecialType
             && IsReferenceType == other.IsReferenceType
         ;
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = Name.GetHashCode();
         hashCode = (hashCode * 397) ^ ArgumentName.GetHashCode();
         hashCode = (hashCode * 397) ^ TypeFullyQualifiedWithNullability.GetHashCode();
         hashCode = (hashCode * 397) ^ IsStatic.GetHashCode();
         hashCode = (hashCode * 397) ^ SpecialType.GetHashCode();
         hashCode = (hashCode * 397) ^ IsReferenceType.GetHashCode();

         return hashCode;
      }
   }
}

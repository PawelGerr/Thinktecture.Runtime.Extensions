using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

public sealed class DefaultMemberState : IMemberState, IEquatable<DefaultMemberState>
{
   private readonly ITypeSymbol _type;

   public string Name { get; }
   public string ArgumentName { get; }

   public string TypeFullyQualified { get; }
   public string TypeFullyQualifiedNullable { get; }
   public string TypeFullyQualifiedNullAnnotated => _type.IsReferenceType ? TypeFullyQualifiedNullable : TypeFullyQualified;
   public string TypeFullyQualifiedWithNullability => _type.IsReferenceType && _type.NullableAnnotation == NullableAnnotation.Annotated ? TypeFullyQualifiedNullAnnotated : TypeFullyQualified;
   public string TypeMinimallyQualified { get; }

   public bool IsStatic { get; }
   public SpecialType SpecialType => _type.SpecialType;
   public bool IsReferenceType => _type.IsReferenceType;
   public bool IsNullableStruct => _type.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T;
   public NullableAnnotation NullableAnnotation => _type.NullableAnnotation;
   public bool IsFormattable { get; }
   public bool IsComparable { get; }
   public bool IsParsable { get; }

   public DefaultMemberState(string name, ITypeSymbol type, string argumentName, bool isStatic)
   {
      _type = type;

      Name = name;
      ArgumentName = argumentName;

      TypeFullyQualified = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
      TypeFullyQualifiedNullable = $"{TypeFullyQualified}?";
      TypeMinimallyQualified = type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

      IsStatic = isStatic;

      foreach (var @interface in type.AllInterfaces)
      {
         if (@interface.IsFormattableInterface())
         {
            IsFormattable = true;
         }
         else if (@interface.IsComparableInterface(_type))
         {
            IsComparable = true;
         }
         else if (@interface.IsParsableInterface(_type))
         {
            IsParsable = true;
         }
      }
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
             && IsFormattable == other.IsFormattable
             && IsComparable == other.IsComparable
             && IsParsable == other.IsParsable;
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
         hashCode = (hashCode * 397) ^ IsFormattable.GetHashCode();
         hashCode = (hashCode * 397) ^ IsComparable.GetHashCode();
         hashCode = (hashCode * 397) ^ IsParsable.GetHashCode();

         return hashCode;
      }
   }
}

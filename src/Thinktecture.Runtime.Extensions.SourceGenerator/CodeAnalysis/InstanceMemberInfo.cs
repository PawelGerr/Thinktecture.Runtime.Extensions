using Microsoft.CodeAnalysis;
using Thinktecture.CodeAnalysis.ValueObjects;

namespace Thinktecture.CodeAnalysis;

public sealed class InstanceMemberInfo : IMemberState, IEquatable<InstanceMemberInfo>
{
   private readonly ITypeSymbol _type;
   private readonly SyntaxToken _identifier;

   public string TypeFullyQualified { get; }
   public string TypeFullyQualifiedNullable { get; }
   public string TypeFullyQualifiedNullAnnotated => _type.IsReferenceType ? TypeFullyQualifiedNullable : TypeFullyQualified;
   public string TypeFullyQualifiedWithNullability => _type.IsReferenceType && _type.NullableAnnotation == NullableAnnotation.Annotated ? TypeFullyQualifiedNullAnnotated : TypeFullyQualified;
   public string TypeMinimallyQualified { get; }
   public NullableAnnotation NullableAnnotation => _type.NullableAnnotation;

   public string Name => _identifier.Text;
   public Accessibility ReadAccessibility { get; }
   public string ArgumentName { get; }
   public bool IsStatic { get; }
   public bool IsReferenceType => _type.IsReferenceType;
   public bool IsReferenceTypeOrNullableStruct => _type.IsReferenceType || _type.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T;
   public bool IsNullableStruct => _type.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T;
   public SpecialType SpecialType => _type.SpecialType;
   public bool IsFormattable { get; }
   public bool IsComparable { get; }
   public bool IsParsable { get; }
   public bool HasAdditionOperators { get; }
   public bool HasSubtractionOperators { get; }
   public bool HasMultiplyOperators { get; }
   public bool HasDivisionOperators { get; }
   public bool HasComparisonOperators { get; }

   public ValueObjectMemberSettings ValueObjectMemberSettings { get; }

   private InstanceMemberInfo(
      ISymbol member,
      ITypeSymbol type,
      SyntaxToken identifier,
      Accessibility readAccessibility,
      bool isStatic)
   {
      _type = type;
      _identifier = identifier;

      TypeFullyQualified = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
      TypeFullyQualifiedNullable = $"{TypeFullyQualified}?";
      TypeMinimallyQualified = type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

      ArgumentName = identifier.Text.MakeArgumentName();

      ReadAccessibility = readAccessibility;
      IsStatic = isStatic;
      ValueObjectMemberSettings = ValueObjectMemberSettings.Create(member, type);

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
         else if (@interface.IsIAdditionOperators(_type))
         {
            HasAdditionOperators = true;
         }
         else if (@interface.IsISubtractionOperators(_type))
         {
            HasSubtractionOperators = true;
         }
         else if (@interface.IsIMultiplyOperators(_type))
         {
            HasMultiplyOperators = true;
         }
         else if (@interface.IsIDivisionOperators(_type))
         {
            HasDivisionOperators = true;
         }
         else if (@interface.IsIComparisonOperators(_type))
         {
            HasComparisonOperators = true;
         }
      }
   }

   public static InstanceMemberInfo CreateFrom(IFieldSymbol field, CancellationToken cancellationToken)
   {
      return new(field, field.Type, field.GetIdentifier(cancellationToken), field.DeclaredAccessibility, field.IsStatic);
   }

   public static InstanceMemberInfo CreateFrom(IPropertySymbol property, CancellationToken cancellationToken)
   {
      return new(property, property.Type, property.GetIdentifier(cancellationToken), property.DeclaredAccessibility, property.IsStatic);
   }

   public Location GetIdentifierLocation()
   {
      return _identifier.GetLocation();
   }

   public override bool Equals(object? obj)
   {
      return obj is InstanceMemberInfo other && Equals(other);
   }

   public bool Equals(IMemberState? obj)
   {
      return obj is InstanceMemberInfo other && Equals(other);
   }

   public bool Equals(InstanceMemberInfo? other)
   {
      if (ReferenceEquals(null, other))
         return false;
      if (ReferenceEquals(this, other))
         return true;

      return TypeFullyQualifiedWithNullability == other.TypeFullyQualifiedWithNullability
             && Name == other.Name
             && ReadAccessibility == other.ReadAccessibility
             && IsStatic == other.IsStatic
             && IsReferenceType == other.IsReferenceType
             && _type.OriginalDefinition.SpecialType == other._type.OriginalDefinition.SpecialType
             && SpecialType == other.SpecialType
             && IsFormattable == other.IsFormattable
             && IsComparable == other.IsComparable
             && IsParsable == other.IsParsable
             && HasAdditionOperators == other.HasAdditionOperators
             && HasSubtractionOperators == other.HasSubtractionOperators
             && HasMultiplyOperators == other.HasMultiplyOperators
             && HasDivisionOperators == other.HasDivisionOperators
             && HasComparisonOperators == other.HasComparisonOperators
             && ValueObjectMemberSettings.Equals(other.ValueObjectMemberSettings);
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = TypeFullyQualifiedWithNullability.GetHashCode();
         hashCode = (hashCode * 397) ^ Name.GetHashCode();
         hashCode = (hashCode * 397) ^ (int)ReadAccessibility;
         hashCode = (hashCode * 397) ^ IsStatic.GetHashCode();
         hashCode = (hashCode * 397) ^ IsReferenceType.GetHashCode();
         hashCode = (hashCode * 397) ^ _type.OriginalDefinition.SpecialType.GetHashCode();
         hashCode = (hashCode * 397) ^ SpecialType.GetHashCode();
         hashCode = (hashCode * 397) ^ IsFormattable.GetHashCode();
         hashCode = (hashCode * 397) ^ IsComparable.GetHashCode();
         hashCode = (hashCode * 397) ^ IsParsable.GetHashCode();
         hashCode = (hashCode * 397) ^ HasAdditionOperators.GetHashCode();
         hashCode = (hashCode * 397) ^ HasSubtractionOperators.GetHashCode();
         hashCode = (hashCode * 397) ^ HasMultiplyOperators.GetHashCode();
         hashCode = (hashCode * 397) ^ HasDivisionOperators.GetHashCode();
         hashCode = (hashCode * 397) ^ HasComparisonOperators.GetHashCode();
         hashCode = (hashCode * 397) ^ ValueObjectMemberSettings.GetHashCode();

         return hashCode;
      }
   }
}

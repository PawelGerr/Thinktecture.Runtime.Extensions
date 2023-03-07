using Microsoft.CodeAnalysis;
using Thinktecture.CodeAnalysis.ValueObjects;

namespace Thinktecture.CodeAnalysis;

public sealed class InstanceMemberInfo : IMemberState, IEquatable<InstanceMemberInfo>
{
   private readonly ITypedMemberState _typedMemberState;
   private readonly SyntaxToken _identifier;

   private string? _argumentName;
   public string ArgumentName => _argumentName ??= Name.MakeArgumentName();

   public string Name { get; }
   public bool IsStatic { get; }
   public Accessibility ReadAccessibility { get; }
   public ValueObjectMemberSettings ValueObjectMemberSettings { get; }

   public SpecialType SpecialType => _typedMemberState.SpecialType;
   public string TypeFullyQualified => _typedMemberState.TypeFullyQualified;
   public string TypeFullyQualifiedNullAnnotated => _typedMemberState.TypeFullyQualifiedNullAnnotated;
   public string TypeFullyQualifiedWithNullability => _typedMemberState.TypeFullyQualifiedWithNullability;
   public string TypeMinimallyQualified => _typedMemberState.TypeMinimallyQualified;
   public string TypeFullyQualifiedNullable => _typedMemberState.TypeFullyQualifiedNullable;
   public bool IsReferenceType => _typedMemberState.IsReferenceType;
   public bool IsReferenceTypeOrNullableStruct => _typedMemberState.IsReferenceTypeOrNullableStruct;
   public bool IsFormattable => _typedMemberState.IsFormattable;
   public bool IsComparable => _typedMemberState.IsComparable;
   public bool IsParsable => _typedMemberState.IsParsable;
   public bool IsNullableStruct => _typedMemberState.IsNullableStruct;
   public NullableAnnotation NullableAnnotation => _typedMemberState.NullableAnnotation;
   public bool HasComparisonOperators => _typedMemberState.HasComparisonOperators;
   public bool HasAdditionOperators => _typedMemberState.HasAdditionOperators;
   public bool HasSubtractionOperators => _typedMemberState.HasAdditionOperators;
   public bool HasMultiplyOperators => _typedMemberState.HasAdditionOperators;
   public bool HasDivisionOperators => _typedMemberState.HasAdditionOperators;

   private InstanceMemberInfo(
      ITypedMemberState typedMemberState,
      ISymbol member,
      ITypeSymbol type,
      string name,
      SyntaxToken identifier,
      Accessibility readAccessibility,
      bool isStatic)
   {
      _typedMemberState = typedMemberState;
      _identifier = identifier;

      Name = name;
      ReadAccessibility = readAccessibility;
      IsStatic = isStatic;
      ValueObjectMemberSettings = ValueObjectMemberSettings.Create(member, type);
   }

   public static InstanceMemberInfo CreateFrom(IFieldSymbol field, CancellationToken cancellationToken)
   {
      return new(TypedMemberState.GetOrCreate(field.Type), field, field.Type, field.Name, field.GetIdentifier(cancellationToken), field.DeclaredAccessibility, field.IsStatic);
   }

   public static InstanceMemberInfo CreateFrom(IPropertySymbol property, CancellationToken cancellationToken)
   {
      return new(TypedMemberState.GetOrCreate(property.Type), property, property.Type, property.Name, property.GetIdentifier(cancellationToken), property.DeclaredAccessibility, property.IsStatic);
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

      return _typedMemberState.Equals(other._typedMemberState)
             && Name == other.Name
             && ReadAccessibility == other.ReadAccessibility
             && IsStatic == other.IsStatic
             && ValueObjectMemberSettings.Equals(other.ValueObjectMemberSettings);
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = _typedMemberState.GetHashCode();
         hashCode = (hashCode * 397) ^ Name.GetHashCode();
         hashCode = (hashCode * 397) ^ (int)ReadAccessibility;
         hashCode = (hashCode * 397) ^ IsStatic.GetHashCode();
         hashCode = (hashCode * 397) ^ ValueObjectMemberSettings.GetHashCode();

         return hashCode;
      }
   }
}
